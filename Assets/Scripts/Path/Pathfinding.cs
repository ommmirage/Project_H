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

    // List<Hex> FindPath(int startX, int startY, int endX, int endY)
    // {
    //     Hex startHex = hexMap.GetHexAt(startX, startY);

    //     openList = new List<Hex> { startHex };
    //     closedList = new List<Hex>();

    //     for (int x = 0; x < hexMap.Width; x++)
    //     {
    //         for (int y = 0; y < hexMap.Height; y++)
    //         {
    //             Hex hex = hexMap.GetHexAt(x, y);
    //         }
    //     }
    // }
}

