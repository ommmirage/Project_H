using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Hex[,] Hexes;
    public Unit unit;

    public GameData(HexMap hexMap)
    {
        Hexes = hexMap.Hexes.Clone() as Hex[,];
        unit = new Knight(hexMap);
        unit.SetHex(Hexes[1,1]);
    }
}
