using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

public class SaveSystem
{
    HexMap hexMap;
    Hex[,] hexes;
    Serializable2d<Hex>[] serializableArray;
    GameData gameData;

    [System.Serializable]
    struct Serializable2d<Hex>
    {
        public int Dimension1;
        public int Dimension2;
        public Hex hex;

        public Serializable2d(int x, int y, Hex hex)
        {
            Dimension1 = x;
            Dimension2 = y;
            this.hex = hex;
        }
    }

    public SaveSystem()
    {
        hexMap = Object.FindObjectOfType<HexMap>();
    }

    public void SaveGame(HexMap hexMap)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Knight));

        gameData = new GameData(hexMap);

        // BinaryFormatter formatter  = new BinaryFormatter();

        string path = Application.persistentDataPath + "/save.sav";
        // FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
        TextWriter writer = new StreamWriter(path);

        // formatter.Serialize(stream, geoData);
        // stream.Close();

        xmlSerializer.Serialize(writer, new Knight(hexMap));
        // xmlSerializer.Serialize(writer, new Unit());
        writer.Close();

        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/save.sav";
        if (File.Exists(path))
        {
            // BinaryFormatter formatter = new BinaryFormatter();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Knight));
            FileStream stream = new FileStream(path, FileMode.Open);

            Knight knight = xmlSerializer.Deserialize(stream) as Knight;
            // Unit unit = xmlSerializer.Deserialize(stream) as Unit;
            stream.Close();

            Debug.Log("Load ok");
        }
        else
        {
            Debug.Log("Save file not found in " + path);
        }
    }

    public void OnBeforeSerialize()
    {
        // Convert unserializable 2d array into serializable
        int width = hexMap.Width;
        int height = hexMap.Height;

        serializableArray = new Serializable2d<Hex>[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                serializableArray[x * y] = new Serializable2d<Hex>(x, y, hexMap.Hexes[x, y]);
            }
        }
    }

    public void OnAfterDeserialize()
    {
        // Convert the serializable array into 2d
        hexes = new Hex[hexMap.Width, hexMap.Height];
        foreach (Serializable2d<Hex> hexWithIds in serializableArray)
        {
            int x = hexWithIds.Dimension1;
            int y = hexWithIds.Dimension2;
            Hex hex = hexWithIds.hex;

            hex.SetHexMap(hexMap);

            hexes[x, y] = hex;
        }

        foreach (Unit unit in gameData.Units)
            unit.SetHexMap(hexMap);

        hexMap.LoadMap(gameData, hexes);
    }
}


