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
        unit.PreparePathMap();

        PathHex startPathHex = unit.GetPathHex(startHex);
        PathHex endPathHex = unit.GetPathHex(endHex);

        Queue<PathHex> openQueue = new Queue<PathHex>();
        List<PathHex> closedList = new List<PathHex>();

        openQueue.Enqueue(startPathHex);

        int movesRemaining = unit.MovesRemaining;

        startPathHex.GCost = 0;

        while (openQueue.Count > 0)
        {
            PathHex currentPathHex = openQueue.Peek();

            if (currentPathHex == endPathHex)
                return GetPathToEndHex(unit, endPathHex);

            openQueue.Dequeue();
            closedList.Add(currentPathHex);

            foreach (PathHex neighbour in GetNeighborList(unit, currentPathHex))
            {
                if (closedList.Contains(neighbour))
                    continue;

                if (!neighbour.IsWalkable)
                {
                    closedList.Add(neighbour);
                    continue;
                }

                float turnsToNeighbour = CalculateTurnsToNeighbour(
                    currentPathHex, neighbour, unit.Moves, movesRemaining);
                    
                if (currentPathHex.GCost + turnsToNeighbour < neighbour.GCost)
                {
                    neighbour.CameFromPathHex = currentPathHex;
                    neighbour.GCost = currentPathHex.GCost + turnsToNeighbour;

                    if (!openQueue.Contains(neighbour))
                    {
                        openQueue.Enqueue(neighbour);
                    }

                    movesRemaining = CalculateMovesRemaining(unit, movesRemaining, currentPathHex, neighbour);                
                }
            }
        }
        return null;
    }

    float CalculateTurnsToNeighbour(PathHex hex, PathHex neighbour, 
                                    int moves, int movesRemaining)
    {
        float turnsToNeighbour;

        if ((hex.Elevation > 0) && (neighbour.Elevation < 0))
        {
            turnsToNeighbour = 1;
        }
        else if ((hex.Elevation < 0) && (neighbour.Elevation > 0))
        {
            turnsToNeighbour = 1;
        }
        else if (movesRemaining >= neighbour.MovementCost)
        {
            turnsToNeighbour = (float)neighbour.MovementCost / moves;
        }
        else
        {
            turnsToNeighbour = 1;
        }

        return turnsToNeighbour;
    }

    int CalculateMovesRemaining(Unit unit, int movesRemaining, PathHex currentPathHex, PathHex neighbour)
    {
        if (unit.Type == "land")
        {
            if ((currentPathHex.Elevation > 0) && (neighbour.Elevation < 0))
            {
                movesRemaining = unit.NavalMoves;
                neighbour.Embark = true;
            }
            else if ((currentPathHex.Elevation < 0) && (neighbour.Elevation > 0))
            {
                movesRemaining = unit.Moves;
                neighbour.Embark = true;
            }
            else
            {
                movesRemaining -= neighbour.MovementCost;

                if (movesRemaining <= 0)
                {
                    if (neighbour.Elevation < 0)
                        movesRemaining = unit.NavalMoves;
                    if (neighbour.Elevation > 0)
                        movesRemaining = unit.Moves;
                }
            }
        }

        return movesRemaining;
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
        if (pathLinked.Count == 0)
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
        ClearPath(path);
        path = FindPath(unit, unit.GetHex(), endHex);
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

