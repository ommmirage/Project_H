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
        hexMap = GameObject.FindObjectOfType<HexMap>();
    }

    public List<Hex> FindPath(int startX, int startY, int endX, int endY)
    {
        Hex startHex = hexMap.GetHexAt(startX, startY);
        Hex endHex = hexMap.GetHexAt(endX, endY);

        openList = new List<Hex> { startHex };
        closedList = new List<Hex>();

        for (int x = 0; x < hexMap.Width; x++)
        {
            for (int y = 0; y < hexMap.Height; y++)
            {
                Hex hex = hexMap.GetHexAt(x, y);
                hex.CalculateFCost();
            }
        }

        startHex.GCost = 0;
        startHex.HCost = CalculateDistanceCost(startHex, endHex);
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

            foreach (Hex neighbor in GetNeighborList(currentHex))
            {
                if (closedList.Contains(neighbor)) 
                {
                    continue;
                }

                // предварительный
                int tentativeGCost = currentHex.GCost + hexMap.Distance(currentHex, neighbor);
                if (tentativeGCost < neighbor.GCost)
                {
                    neighbor.CameFromHex = currentHex;
                    neighbor.GCost = tentativeGCost;
                    neighbor.HCost = hexMap.Distance(neighbor, endHex);
                    neighbor.CalculateFCost();

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    int CalculateDistanceCost(Hex a, Hex b)
    {
        int dq = Mathf.Abs(a.Q - b.Q);
        if (dq > hexMap.Width / 2)
        {
            dq = dq - hexMap.Width / 2;
        }
        return ( + Mathf.Abs(a.R - b.R) + Mathf.Abs(a.S - b.S)) / 2;
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

