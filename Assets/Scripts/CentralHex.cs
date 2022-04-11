using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralHex : Hex
{
    int range;
    public int Range { get { return range; } }

    public CentralHex(int q, int r, int range): base(q, r)
    {
        this.range = range;
    }

    public CentralHex(Hex hex, int range):
        base(hex.Q, hex.R)
    {
        this.range = range;
    }
}
