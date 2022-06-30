using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveGeo(HexMap hexMap)
    {
        BinaryFormatter formatter  = new BinaryFormatter();

        string path = Application.persistentDataPath + "/save.sav";
        FileStream stream = new FileStream(path, FileMode.OpenOrCreate);

        GameData geoData = new GameData(hexMap);

        formatter.Serialize(stream, geoData);
        stream.Close();

        Debug.Log("Game Saved");
    }

    public static void LoadGeo()
    {
        string path = Application.persistentDataPath + "/save.sav";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData gameData = formatter.Deserialize(stream) as GameData;
            stream.Close();

            Debug.Log("Load ok");
            // Debug.Log(gameData.Units[0].GetHex());

            HexMap hexMap = Object.FindObjectOfType<HexMap>();

            SetHexMapToHexesAndUnits(gameData, hexMap);
            hexMap.LoadMap(gameData);
        }
        else
        {
            Debug.Log("Save file not found in " + path);
        }
    }

    static void SetHexMapToHexesAndUnits(GameData gameData, HexMap hexMap)
    {
        for (int x = 0; x < hexMap.Width; x++)
        {
            for (int y = 0; y < hexMap.Height; y++)
            {
                gameData.Hexes[x, y].SetHexMap(hexMap);
            }
        }

        // foreach (Unit unit in gameData.Units)
        //     unit.Set
    }
}


