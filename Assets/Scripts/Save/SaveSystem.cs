using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;

public class SaveSystem
{
    HexMap hexMap;

    public SaveSystem()
    {
        hexMap = Object.FindObjectOfType<HexMap>();
    }

    public void SaveGame(HexMap hexMap)
    {
        // XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameData));

        GameData gameData = new GameData(hexMap);

        string path = Application.persistentDataPath + "/save.sav";
        // TextWriter writer = new StreamWriter(path);
        FileStream writer = new FileStream(path, FileMode.Create);

        DataContractSerializer ser = new DataContractSerializer(typeof(GameData));

        ser.WriteObject(writer, gameData);
        writer.Close();

        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/save.sav";
        if (File.Exists(path))
        {
            // XmlSerializer xmlSerializer = new XmlSerializer(typeof(GameData));
            FileStream stream = new FileStream(path, FileMode.Open);
            DataContractSerializer ser = new DataContractSerializer(typeof(GameData));

            GameData gameData = ser.ReadObject(stream) as GameData;
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
        Hex[,] hexes = FillHexes(gameData);

        foreach (Unit unit in gameData.Units)
            unit.SetHexMap(hexMap);

        hexMap.LoadMap(gameData, hexes);
    }

    Hex[,] FillHexes(GameData gameData)
    {
        // Convert the serializable array into 2d
        Hex[,] hexes = new Hex[hexMap.Width, hexMap.Height];
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

            hexes[x, y] = hex;
        }
        return hexes;
    }
}


