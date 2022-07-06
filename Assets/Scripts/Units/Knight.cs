using System.Runtime.Serialization;

[DataContract]
public class Knight : Unit
{
    // GameObject prefab;
    public Knight() {}

    public Knight(HexMap hexMap) : base(hexMap)
    {
        hp = 100;
        strength = 8;
        IsEmbarked = false;
        type = "land";
        moves = 3;
        navalMoves = 2;
        // prefab = hexMap.UnitKnightPrefab;
    }
}
