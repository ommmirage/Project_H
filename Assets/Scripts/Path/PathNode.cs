using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    HexMap hexMap;
    int x;
    int y;
    int gCost;

    public PathNode(Hex hex)
    {
        x = hex.Q;
        y = hex.R;
    }
}