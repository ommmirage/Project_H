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

    public List<PathHex> FindPath(Unit unit, Hex startHex, Hex endHex)
    {
        unit.PreparePathMap();

        PathHex startPathHex = unit.GetPathHex(startHex);
        PathHex endPathHex = unit.GetPathHex(endHex);

        List<PathHex> openList = new List<PathHex> { startPathHex };
        List<PathHex> closedList = new List<PathHex>();

        int movesRemaining = unit.MovesRemaining;

        startPathHex.GCost = 0;

        while (openList.Count > 0)
        {
            PathHex currentPathHex = GetLowestGCostHex(openList);

            if (currentPathHex == endPathHex)
                return GetPathToEndHex(unit, endPathHex);

            openList.Remove(currentPathHex);
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

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
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

    PathHex GetLowestGCostHex(List<PathHex> pathHexList)
    {
        PathHex lowestGCostHex = pathHexList[0];
        for (int i = 0; i < pathHexList.Count; i++)
        {
            if (pathHexList[i].GCost < lowestGCostHex.GCost)
            {
                lowestGCostHex = pathHexList[i];
            }
        }
        return lowestGCostHex;
    }

    List<PathHex> GetPathToEndHex(Unit unit, PathHex endPathHex)
    {
        List<PathHex> path = new List<PathHex>();

        path.Add(endPathHex);

        PathHex currentPathHex = endPathHex;
        while ( (currentPathHex.CameFromPathHex != null) && 
                (currentPathHex != currentPathHex.CameFromPathHex) )
        {
            path.Add(currentPathHex.CameFromPathHex);
            currentPathHex = currentPathHex.CameFromPathHex;
        }
        path.Reverse();

        unit.Path = new LinkedList<PathHex>(path);
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

    public void DrawPath(List<PathHex> path, Unit unit)
	{
        if (path.Count == 0)
            return;

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

	public void ClearPath(List<PathHex> path)
	{
        if (path != null)
        {
            foreach (PathHex pathHex in path)
            {
                hexMap.GetHexAt(pathHex).Clear();
            }
        }
    }

    public int Distance(PathHex a, PathHex b)
    {
        int dq = Mathf.Abs(a.Q - b.Q);
        if (dq > hexMap.Width / 2)
        {
            dq = Mathf.Abs(hexMap.Width - dq);
        }

        int ds = Mathf.Abs(a.S - b.S);
        if (ds > hexMap.Width / 2)
        {
            ds = Mathf.Abs(hexMap.Width - ds);
        }

        return (dq + Mathf.Abs(a.R - b.R) + ds) / 2;
    }

    public List<PathHex> RedrawPath(List<PathHex> path, Unit unit, Hex endHex)
    {
        ClearPath(path);
        path = FindPath(unit, unit.GetHex(), endHex);
        DrawPath(path, unit);

        return path;
    }
}

