using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Unit
{
    public Knight(HexMap hexMap) : base(hexMap)
    {
        hp = 100;
        strength = 8;
        IsEmbarked = false;
        type = "land";
        moves = 3;
        navalMoves = 2;
    }
}
