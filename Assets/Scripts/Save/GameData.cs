using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Hex[,] Hexes;
    public List<Unit> Units;

    public GameData(HexMap hexMap)
    {
        Hexes = hexMap.Hexes.Clone() as Hex[,];
        // Units = hexMap.Units;
    }
}
