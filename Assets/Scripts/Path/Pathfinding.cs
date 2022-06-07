using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    List<Hex> openList;
    List<Hex> closedList;
    HexMap hexMap;


    public Pathfinding()
    {
        hexMap = Object.FindObjectOfType<HexMap>();
    }

    public List<Hex> FindPath(Unit unit, Hex startHex, Hex endHex)
    {
        openList = new List<Hex> { startHex };
        closedList = new List<Hex>();

        PrepareHexMap(unit.Type);

        startHex.GCost = 0;
        startHex.HCost = hexMap.Distance(startHex, endHex);
        startHex.CalculateFCost();

        while (openList.Count > 0)
        {
            Hex currentHex = GetLowestFCostHex(openList);
            if (currentHex == endHex)
            {
                return CalculatePath(endHex);
            }

            openList.Remove(currentHex);
            closedList.Add(currentHex);

            foreach (Hex neighbour in GetNeighborList(currentHex))
            {
                if (closedList.Contains(neighbour))
                {
                    continue;
                }
                if (!neighbour.IsWalkable)
                {
                    closedList.Add(neighbour);
                    continue;
                }

                int movesRemaining = unit.MovesRemaining;
                float turnsToNeighbour = CalculateTurnsToNeighbour(unit.Moves, ref movesRemaining, neighbour);

                if (currentHex.GCost + turnsToNeighbour < neighbour.GCost)
                {
                    neighbour.CameFromHex = currentHex;
                    neighbour.GCost = currentHex.GCost + turnsToNeighbour;
                    neighbour.HCost = hexMap.Distance(neighbour, endHex);
                    neighbour.CalculateFCost();
                    if (unit.MovesRemaining == 0)
                    {
                        unit.MovesRemaining = unit.Moves;
                    }

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }

        return null;
    }

    void PrepareHexMap(string unitType)
    {
        for (int x = 0; x < hexMap.Width; x++)
        {
            for (int y = 0; y < hexMap.Height; y++)
            {
                Hex hex = hexMap.GetHexAt(x, y);
                hex.GCost = int.MaxValue;
                hex.FCost = int.MaxValue;
                hex.CameFromHex = null;
                hex.SetWalkable(unitType);
            }
        }
    }

    float CalculateTurnsToNeighbour(int moves, ref int movesRemaining, Hex neighbour)
    {
        float turnsToNeighbour;
        if (movesRemaining >= neighbour.MovementCost)
        {
            turnsToNeighbour = neighbour.MovementCost / moves;
        }
        else
        {
            turnsToNeighbour = 1;
        }

        movesRemaining -= neighbour.MovementCost;

        if (movesRemaining <= 0)
            movesRemaining = moves;

        return turnsToNeighbour;
    }

    Hex GetLowestFCostHex(List<Hex> pathHexList)
    {
        Hex lowestFCostHex = pathHexList[0];
        for (int i = 0; i < pathHexList.Count; i++)
        {
            if (pathHexList[i].FCost < lowestFCostHex.FCost)
            {
                lowestFCostHex = pathHexList[i];
            }
        }
        return lowestFCostHex;
    }

    List<Hex> CalculatePath(Hex endHex)
    {
        List<Hex> path = new List<Hex>();
        path.Add(endHex);
        Hex currentHex = endHex;
        while (currentHex.CameFromHex != null)
        {
            path.Add(currentHex.CameFromHex);
            currentHex = currentHex.CameFromHex;
        }
        path.Reverse();

        return path;
    }

    List<Hex> GetNeighborList(Hex hex)
    {
        List<Hex> neighborList = new List<Hex>();
        int q = hex.Q;
        int r = hex.R;

        if (r + 1 < hexMap.Height)
        {
            neighborList.Add(hexMap.GetHexAt(q, r + 1));
            neighborList.Add(hexMap.GetHexAt(q - 1, r + 1));
        }
        if (r != 0)
        {
            neighborList.Add(hexMap.GetHexAt(q, r - 1));
            neighborList.Add(hexMap.GetHexAt(q + 1, r - 1));
        }
        neighborList.Add(hexMap.GetHexAt(q + 1, r));
        neighborList.Add(hexMap.GetHexAt(q - 1, r));

        return neighborList;
    }

    public void DrawPath(List<Hex> path, Unit unit)
	{
		for (int i = 0; i < path.Count - 1; i++)
		{
			Hex hex = path[i];
			Hex nextHex = path[i + 1];
            Transform hexPathLine;
            Transform nextHexPathLine;

            int movesRemaining = unit.MovesRemaining;
            
            movesRemaining -= hex.MovementCost;
            if (movesRemaining <= 0)
            {
                // Choose short lines
                hexPathLine = hexMap.HexToGameObjectDictionary[hex].gameObject.transform.GetChild(5);
                nextHexPathLine = hexMap.HexToGameObjectDictionary[nextHex].gameObject.transform.GetChild(5);

                movesRemaining = unit.Moves;
            }
            else
            {
                // Choose long lines
                hexPathLine = hexMap.HexToGameObjectDictionary[hex].gameObject.transform.GetChild(4);
                nextHexPathLine = hexMap.HexToGameObjectDictionary[nextHex].gameObject.transform.GetChild(4);
            }

            if (nextHex.Q == hex.Q)
            {
                if (nextHex.R > hex.R)
                {
                    if ((hex.Q == 0 && nextHex.Q == 84) || (hex.Q == 84 && nextHex.Q == 0))
                    {
                        hexPathLine.GetChild(3).gameObject.SetActive(true);
                        nextHexPathLine.GetChild(2).gameObject.SetActive(true);
                    }
                    else
                    {
                        hexPathLine.GetChild(2).gameObject.SetActive(true);
                        nextHexPathLine.GetChild(3).gameObject.SetActive(true);
                    }
                }
                else
                {
                    if ((hex.Q == 0 && nextHex.Q == 84) || (hex.Q == 84 && nextHex.Q == 0))
                    {
                        hexPathLine.GetChild(2).gameObject.SetActive(true);
                        nextHexPathLine.GetChild(3).gameObject.SetActive(true);
                    }
                    else
                    {
                        hexPathLine.GetChild(3).gameObject.SetActive(true);
                        nextHexPathLine.GetChild(2).gameObject.SetActive(true);
                    }
                }
            }
            else if (nextHex.R == hex.R)
            {
                if (nextHex.Q > hex.Q)
                {
                    if ((hex.Q == 0 && nextHex.Q == 84) || (hex.Q == 84 && nextHex.Q == 0))
                    {
                        hexPathLine.GetChild(5).gameObject.SetActive(true);
                        nextHexPathLine.GetChild(4).gameObject.SetActive(true);
                    }
                    else
                    {
                        hexPathLine.GetChild(4).gameObject.SetActive(true);
                        nextHexPathLine.GetChild(5).gameObject.SetActive(true);
                    }
                }
                else
                {
                    if ((hex.Q == 0 && nextHex.Q == 84) || (hex.Q == 84 && nextHex.Q == 0))
                    {
                        hexPathLine.GetChild(4).gameObject.SetActive(true);
                        nextHexPathLine.GetChild(5).gameObject.SetActive(true);
                    }
                    else
                    {
                        hexPathLine.GetChild(5).gameObject.SetActive(true);
                        nextHexPathLine.GetChild(4).gameObject.SetActive(true);
                    }
                }
            }
            else if (nextHex.Q < hex.Q)
            {
                if ((hex.Q == 0 && nextHex.Q == 84) || (hex.Q == 84 && nextHex.Q == 0))
                {
                    hexPathLine.GetChild(1).gameObject.SetActive(true);
                    nextHexPathLine.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    hexPathLine.GetChild(0).gameObject.SetActive(true);
                    nextHexPathLine.GetChild(1).gameObject.SetActive(true);
                }
            }
            else
            {
                if ((hex.Q == 0 && nextHex.Q == 84) || (hex.Q == 84 && nextHex.Q == 0))
                {
                    hexPathLine.GetChild(0).gameObject.SetActive(true);
                    nextHexPathLine.GetChild(1).gameObject.SetActive(true);
                }
                else
                {
                    hexPathLine.GetChild(1).gameObject.SetActive(true);
                    nextHexPathLine.GetChild(0).gameObject.SetActive(true);
                }
            }
		}
	}

	public void ClearPath(List<Hex> path)
	{
		foreach (Hex hex in path)
		{
			Transform longLines = hexMap.HexToGameObjectDictionary[hex].gameObject.transform.GetChild(4);
			foreach (Transform road in longLines)
			{
				road.gameObject.SetActive(false);
			}

            Transform shortLines = hexMap.HexToGameObjectDictionary[hex].gameObject.transform.GetChild(5);
			foreach (Transform road in longLines)
			{
				road.gameObject.SetActive(false);
			}
		}
	}
}

