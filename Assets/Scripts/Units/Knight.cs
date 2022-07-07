using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class Knight : Unit
{
    public Knight() {}

    public Knight(HexMap hexMap) : base(hexMap)
    {
        // Debug.Log(hexMap);

        hp = 100;
        strength = 8;
        IsEmbarked = false;
        type = "land";
        moves = 3;
        navalMoves = 2;
    }
}
