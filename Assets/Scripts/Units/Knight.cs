using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Unit
{
    public Knight()
    {
        hp = 100;
        strength = 8;
        moves = 2;
        MovesRemaining = moves;
        type = "land";
    }
}
