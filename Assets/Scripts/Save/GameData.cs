using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<Unit> Units;

    public GameData()
    {

    }

    public GameData(HexMap hexMap)
    {
        Units = hexMap.Units;
    }
}
