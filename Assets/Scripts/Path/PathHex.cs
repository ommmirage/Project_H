using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathHex
{
    // readonly means that variable is only set in the contructor
    public readonly int Q;
    public readonly int R;
    public readonly int S;

    public float Elevation;

    public int MovementCost;

    // Walking cost from the start hex
    public float GCost = int.MaxValue;
    // Heuristic cost to reach End Hex
    public float HCost;
    public float FCost = int.MaxValue;
    public PathHex CameFromPathHex = null;
    public int turns;
    public bool IsWalkable = true;
    public bool Embark = false;

    public PathHex(Hex hex)
    {
        Q = hex.Q;
        R = hex.R;
        S = -(Q + R);
        Elevation = hex.Elevation;
        MovementCost = hex.MovementCost;
    }

    public void CalculateFCost()
    {
        FCost = GCost + HCost;
    }

    public override string ToString()
    {
        return Q + ", "+ R;
    }
}
