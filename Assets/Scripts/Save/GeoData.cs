using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeoData
{
    public Hex[,] Hexes;

    public GeoData(Hex[,] hexes)
    {
        this.Hexes = hexes.Clone() as Hex[,];
    }
}
