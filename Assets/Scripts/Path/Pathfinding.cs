using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    HexMap hexMap;

    public Pathfinding()
    {
        hexMap = Object.FindObjectOfType<HexMap>();
    }

    public LinkedList<PathHex> FindPath(Unit unit, Hex startHex, Hex endHex)
    {
        // Debug.Log("");
        // Debug.Log("Find path");
        // Debug.Log("");
        
        unit.PreparePathMap();

        PathHex startPathHex = unit.GetPathHex(startHex);
        PathHex endPathHex = unit.GetPathHex(endHex);

        Queue<PathHex> openQueue = new Queue<PathHex>();
        List<PathHex> closedList = new List<PathHex>();

        openQueue.Enqueue(startPathHex);

        startPathHex.MovesRemaining = unit.MovesRemaining;

        startPathHex.GCost = 0;

        int destinatedEnd = 0;
        int pathsAvailable = PathsAvailable(unit, endPathHex);

        while ( (openQueue.Count > 0) && (destinatedEnd < pathsAvailable) )
        {
            PathHex currentPathHex = openQueue.Dequeue();

            if (currentPathHex == endPathHex)
            {
                if (openQueue.Count == 1)
                    return GetPathToEndHex(unit, endPathHex);

                openQueue.Enqueue(currentPathHex);
                destinatedEnd++;
                currentPathHex = openQueue.Dequeue();
            }
            
            closedList.Add(currentPathHex);

            // if ( (currentPathHex == unit.GetPathHexAt(84, 16)) 
            //     || (currentPathHex == unit.GetPathHexAt(84, 17)) )
            //     Debug.Log("currentPathHex: " + currentPathHex + "; GCost: " + currentPathHex.GCost);

            foreach (PathHex neighbour in GetNeighborList(unit, currentPathHex))
            {
                if (closedList.Contains(neighbour))
                    continue;

                if (!neighbour.IsWalkable)
                {
                    closedList.Add(neighbour);
                    continue;
                }

                // if (neighbour == unit.GetPathHexAt(0, 16))
                // {
                //     Debug.Log("neighbour: " + neighbour + " " + 
                //         "currentPathHex.MovesRemaining: " + currentPathHex.MovesRemaining);
                // }

                float turnsToNeighbour = CalculateTurnsToNeighbour(currentPathHex, neighbour, unit);

                float newGCost = currentPathHex.GCost + turnsToNeighbour;


                // if ( (neighbour == unit.GetPathHexAt(83, 17)) )//|| (neighbour == unit.GetPathHexAt(84, 16)) )
                // {
                //     Debug.Log("neighbour: " + neighbour + " " + 
                //         "GCost: " + neighbour.GCost + "\n" + 
                //         "turnsToNeighbour: " + turnsToNeighbour + " " +
                //         "currentPathHex.MovesRemaining: " + currentPathHex.MovesRemaining);
                // }

                if ( (newGCost < neighbour.GCost) )
                {
                    neighbour.CameFromPathHex = currentPathHex;
                    neighbour.GCost = newGCost;

                    if (!openQueue.Contains(neighbour))
                    {
                        openQueue.Enqueue(neighbour);
                    }

                    CalculateMovesRemaining(unit, currentPathHex, neighbour);                
                }

                // Debug.Log(neighbour + ". " + "movesRemaining: " + neighbour.MovesRemaining);

            }
        }
        return GetPathToEndHex(unit, endPathHex);
    }

    int SetMovesRemaining(PathHex pathHex, Unit unit)
    {
        if (pathHex.Elevation > 0)
        {
            return unit.Moves;
        }
        else
        {
            return unit.NavalMoves;
        }
    }

    int PathsAvailable(Unit unit, PathHex hex)
    {
        int pathsAvailable = 0;
        foreach (PathHex neighbour in GetNeighborList(unit, hex))
        {
            if (neighbour.IsWalkable)
                pathsAvailable++;
        }

        return pathsAvailable;
    }

    float CalculateTurnsToNeighbour(PathHex hex, PathHex neighbour, Unit unit)
    {
        float turnsToNeighbour;

        if ( (hex.MovesRemaining == 0) && unit.FinishedMove )
        {
            hex.MovesRemaining = SetMovesRemaining(hex, unit);
        }

        if ((hex.Elevation > 0) && (neighbour.Elevation < 0))
        {
            turnsToNeighbour = (float)hex.MovesRemaining / unit.Moves;
        }
        else if ((hex.Elevation < 0) && (neighbour.Elevation > 0))
        {
            turnsToNeighbour = (float)hex.MovesRemaining / unit.NavalMoves;
        }
        else if (hex.MovesRemaining >= neighbour.MovementCost)
        {
            if (hex.Elevation > 0)
            {
                turnsToNeighbour = (float)neighbour.MovementCost / unit.Moves;
            }
            else
            {
                turnsToNeighbour = (float)neighbour.MovementCost / unit.NavalMoves;
            }
        }
        else
        {
            turnsToNeighbour = 1;
        }

        return turnsToNeighbour;
    }

    void CalculateMovesRemaining(Unit unit, PathHex currentPathHex, PathHex neighbour)
    {
        if (unit.Type == "land")
        {
            if ((currentPathHex.Elevation > 0) && (neighbour.Elevation < 0))
            {
                neighbour.MovesRemaining = unit.NavalMoves;
                neighbour.Embark = true;
            }
            else if ((currentPathHex.Elevation < 0) && (neighbour.Elevation > 0))
            {
                neighbour.MovesRemaining = unit.Moves;
                neighbour.Embark = true;
            }
            else
            {
                neighbour.Embark = false;
                neighbour.MovesRemaining = currentPathHex.MovesRemaining - neighbour.MovementCost;

                if (neighbour.MovesRemaining <= 0)
                {
                    if (neighbour.Elevation < 0)
                        neighbour.MovesRemaining = unit.NavalMoves;
                    if (neighbour.Elevation > 0)
                        neighbour.MovesRemaining = unit.Moves;
                }
            }
        }
    }

    LinkedList<PathHex> GetPathToEndHex(Unit unit, PathHex endPathHex)
    {
        LinkedList<PathHex> path = new LinkedList<PathHex>();

        path.AddLast(endPathHex);

        PathHex currentPathHex = endPathHex;
        while ( (currentPathHex.CameFromPathHex != null) && 
                (currentPathHex != currentPathHex.CameFromPathHex) )
        {
            path.AddLast(currentPathHex.CameFromPathHex);
            currentPathHex = currentPathHex.CameFromPathHex;
        }
        Reverse(ref path);

        unit.Path = path;

        return path;
    }

    List<PathHex> GetNeighborList(Unit unit, PathHex hex)
    {
        List<PathHex> neighborList = new List<PathHex>();
        int q = hex.Q;
        int r = hex.R;

        if (r + 1 < hexMap.Height)
        {
            neighborList.Add(unit.GetPathHexAt(q, r + 1));
            neighborList.Add(unit.GetPathHexAt(q - 1, r + 1));
        }
        if (r != 0)
        {
            neighborList.Add(unit.GetPathHexAt(q, r - 1));
            neighborList.Add(unit.GetPathHexAt(q + 1, r - 1));
        }
        neighborList.Add(unit.GetPathHexAt(q + 1, r));
        neighborList.Add(unit.GetPathHexAt(q - 1, r));

        return neighborList;
    }

    public void DrawPath(LinkedList<PathHex> pathLinked, Unit unit)
	{
        if ( (pathLinked == null) || (pathLinked.Count == 0) )
            return;

        List<PathHex> path = new List<PathHex>(pathLinked);

        int movesRemaining = unit.MovesRemaining;
        int movesCount = 0;

		for (int i = 0; i < path.Count - 1; i++)
		{
			PathHex pathHex = path[i];
            PathHex nextPathHex = path[i + 1];
            Hex hex = hexMap.GetHexAt(pathHex);
            Hex nextHex = hexMap.GetHexAt(nextPathHex);
            GameObject hexGameObject = hexMap.HexToGameObjectDictionary[hex].gameObject;
            Transform hexLongLines = hexGameObject.transform.GetChild(4);

            DrawLongLines(hex, nextHex, hexLongLines);

            if (i > 0)
                movesRemaining -= pathHex.MovementCost;

            if (pathHex.Embark)
                movesRemaining = 0;

            if (movesRemaining <= 0)
            {
                movesCount++;

                DrawMovesCount(hexGameObject, hexLongLines, movesCount);

                if (pathHex.Elevation > 0)
                {
                    movesRemaining = unit.Moves;
                }
                else
                {
                    movesRemaining = unit.NavalMoves;
                }
            }
        }

        movesCount++;
        Hex lastHex = hexMap.GetHexAt(path[path.Count - 1]);
        DrawLastMove(movesCount, lastHex);
	}

    void DrawLastMove(float movesCount, Hex hex)
    {
        GameObject hexGameObject = hexMap.HexToGameObjectDictionary[hex].gameObject;
        Transform hexLongLines = hexGameObject.transform.GetChild(4);

        DrawMovesCount(hexGameObject, hexLongLines, movesCount);
    }

    void DrawMovesCount(GameObject hexGameObject, Transform hexLongLines, float movesCount)
    {
        Transform hexShortLines = hexGameObject.transform.GetChild(5);

        for (int i = 0; i < 6; i++)
        {
            GameObject longLineGameObject = hexLongLines.GetChild(i).gameObject;
            if (longLineGameObject.activeSelf)
            {
                longLineGameObject.SetActive(false);
                hexShortLines.GetChild(i).gameObject.SetActive(true);

                GameObject unitMovesGameObject = hexGameObject.transform.GetChild(6).gameObject;
                TextMesh textMesh = unitMovesGameObject.GetComponentInChildren<TextMesh>();
                textMesh.text = movesCount.ToString();
                if (movesCount >= 10)
                {
                    textMesh.characterSize = 0.55f;
                }
                unitMovesGameObject.transform.GetChild(0).gameObject.SetActive(true);           
            }
        }
    }

    void DrawLongLines(Hex hex, Hex nextHex, Transform hexLongLines)
    {
        Transform nextHexLongLines = hexMap.HexToGameObjectDictionary[nextHex].gameObject.transform.GetChild(4);

        if (nextHex.Q == hex.Q)
        {
            if (nextHex.R > hex.R)
            {
                if ((hex.Q == 0 && nextHex.Q == 84) || (hex.Q == 84 && nextHex.Q == 0))
                {
                    hexLongLines.GetChild(3).gameObject.SetActive(true);
                    nextHexLongLines.GetChild(2).gameObject.SetActive(true);
                }
                else
                {
                    hexLongLines.GetChild(2).gameObject.SetActive(true);
                    nextHexLongLines.GetChild(3).gameObject.SetActive(true);
                }
            }
            else
            {
                if ((hex.Q == 0 && nextHex.Q == 84) || (hex.Q == 84 && nextHex.Q == 0))
                {
                    hexLongLines.GetChild(2).gameObject.SetActive(true);
                    nextHexLongLines.GetChild(3).gameObject.SetActive(true);
                }
                else
                {
                    hexLongLines.GetChild(3).gameObject.SetActive(true);
                    nextHexLongLines.GetChild(2).gameObject.SetActive(true);
                }
            }
        }
        else if (nextHex.R == hex.R)
        {
            if (nextHex.Q > hex.Q)
            {
                if ((hex.Q == 0 && nextHex.Q == 84) || (hex.Q == 84 && nextHex.Q == 0))
                {
                    hexLongLines.GetChild(5).gameObject.SetActive(true);
                    nextHexLongLines.GetChild(4).gameObject.SetActive(true);
                }
                else
                {
                    hexLongLines.GetChild(4).gameObject.SetActive(true);
                    nextHexLongLines.GetChild(5).gameObject.SetActive(true);
                }
            }
            else
            {
                if ((hex.Q == 0 && nextHex.Q == 84) || (hex.Q == 84 && nextHex.Q == 0))
                {
                    hexLongLines.GetChild(4).gameObject.SetActive(true);
                    nextHexLongLines.GetChild(5).gameObject.SetActive(true);
                }
                else
                {
                    hexLongLines.GetChild(5).gameObject.SetActive(true);
                    nextHexLongLines.GetChild(4).gameObject.SetActive(true);
                }
            }
        }
        else if (nextHex.Q < hex.Q)
        {
            if ((hex.Q == 0 && nextHex.Q == 84) || (hex.Q == 84 && nextHex.Q == 0))
            {
                hexLongLines.GetChild(1).gameObject.SetActive(true);
                nextHexLongLines.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                hexLongLines.GetChild(0).gameObject.SetActive(true);
                nextHexLongLines.GetChild(1).gameObject.SetActive(true);
            }
        }
        else
        {
            if ((hex.Q == 0 && nextHex.Q == 84) || (hex.Q == 84 && nextHex.Q == 0))
            {
                hexLongLines.GetChild(0).gameObject.SetActive(true);
                nextHexLongLines.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                hexLongLines.GetChild(1).gameObject.SetActive(true);
                nextHexLongLines.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

	public void ClearPath(LinkedList<PathHex> path)
	{
        if (path != null)
        {
            foreach (PathHex pathHex in path)
            {
                hexMap.GetHexAt(pathHex).Clear();
            }
        }
    }

    public LinkedList<PathHex> RedrawPath(LinkedList<PathHex> path, Unit unit, Hex endHex)
    {
        // Debug.Log("old path");
        // foreach (PathHex hex in path)
        // {
        //     Debug.Log(hex);
        // }

        ClearPath(path);
        path = FindPath(unit, unit.GetHex(), endHex);

        // Debug.Log("new path");
        // foreach (PathHex hex in path)
        // {
        //     Debug.Log(hex);
        // }

        DrawPath(path, unit);

        return path;
    }

    void Reverse(ref LinkedList<PathHex> path) 
    {
        LinkedList<PathHex> copyList = new LinkedList<PathHex>();

        LinkedListNode<PathHex> start = path.Last;

        while (start != null)
        {
            copyList.AddLast(start.Value);
            start = start.Previous;
        }

        path = copyList;
    }
}

