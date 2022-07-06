using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

[DataContract]
public class GameData
{
    HexMap hexMap;
    Hex[,] Hexes;

    [DataMember()]
    public List<Unit> Units;

    [DataMember()]
    public Serializable2d<Hex>[] SerializableArray;

    public GameData(){}

    public GameData(HexMap hexMap)
    {
        this.hexMap = hexMap;
        PackHexesToSerializableArray();
        Units = hexMap.Units;
    }

    void PackHexesToSerializableArray()
    {
        // Convert unserializable 2d array into serializable
        int width = hexMap.Width;
        int height = hexMap.Height;

        SerializableArray = new Serializable2d<Hex>[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SerializableArray[y + x * height] = 
                    new Serializable2d<Hex>(x, y, hexMap.Hexes[x, y]);
            }
        }
    }
}
