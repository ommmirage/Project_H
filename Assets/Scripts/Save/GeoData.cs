using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GeoData
{
    public Hex[,] hexes;

    public GeoData(Hex[,] hexes)
    {
        this.hexes = hexes.Clone() as Hex[,];
    }
}
