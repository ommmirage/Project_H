using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;

public class SaveSystem
{
    HexMap hexMap;
    Hex[,] hexes;

    public SaveSystem()
    {
        hexMap = Object.FindObjectOfType<HexMap>();
    }

    public void SaveGame(HexMap hexMap)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameData));

        GameData gameData = new GameData(hexMap);

        // BinaryFormatter formatter  = new BinaryFormatter();

        string path = Application.persistentDataPath + "/save.sav";
        // FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
        TextWriter writer = new StreamWriter(path);

        // formatter.Serialize(stream, geoData);
        // stream.Close();

        xmlSerializer.Serialize(writer, gameData);
        writer.Close();

        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/save.sav";
        if (File.Exists(path))
        {
            // BinaryFormatter formatter = new BinaryFormatter();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameData));
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData gameData = xmlSerializer.Deserialize(stream) as GameData;
            stream.Close();

            OnAfterDeserialize(gameData);

            Debug.Log("Load ok");
        }
        else
        {
            Debug.Log("Save file not found in " + path);
        }
    }

    void OnAfterDeserialize(GameData gameData)
    {
        // Convert the serializable array into 2d
        hexes = new Hex[hexMap.Width, hexMap.Height];
        foreach (Serializable2d<Hex> hexWithIds in gameData.SerializableArray)
        {
            int x = hexWithIds.Dimension1;
            int y = hexWithIds.Dimension2;
            Hex hex = new Hex(hexMap, x, y);
            hex.Elevation = hexWithIds.hex.Elevation;
            hex.Continent = hexWithIds.hex.Continent;
            hex.Territory = hexWithIds.hex.Territory;
            hex.MovementCost = hexWithIds.hex.MovementCost;
            hex.IsForest = hexWithIds.hex.IsForest;

            Debug.Log(hex);

            hexes[x, y] = hex;
        }

        foreach (Unit unit in gameData.Units)
            unit.SetHexMap(hexMap);

        hexMap.LoadMap(gameData, hexes);
    }
}


