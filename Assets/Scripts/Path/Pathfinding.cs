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

        for (int x = 0; x < hexMap.Width; x++)
        {
            for (int y = 0; y < hexMap.Height; y++)
            {
                Hex hex = hexMap.GetHexAt(x, y);
                hex.GCost = int.MaxValue;
                hex.CalculateFCost();
                hex.CameFromHex = null;
                hex.SetWalkable(unit.Type);
            }
        }

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

                // предварительный
                int tentativeGCost = currentHex.GCost + neighbour.MovementCost;

                if (tentativeGCost < neighbour.GCost)
                {
                    neighbour.CameFromHex = currentHex;
                    neighbour.GCost = tentativeGCost;
                    neighbour.HCost = hexMap.Distance(neighbour, endHex);
                    neighbour.CalculateFCost();
                    if (unit.movementRemaining == 0)
                    {
                        unit.movementRemaining = unit.Movement;
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
}

