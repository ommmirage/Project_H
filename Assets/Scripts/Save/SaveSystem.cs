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

            Hex[,] hexes = FillHexes(gameData);

            hexMap.LoadMap(gameData, hexes);

            Debug.Log("Load ok");
        }
        else
        {
            Debug.Log("Save file not found in " + path);
        }
    }

    Hex[,] FillHexes(GameData gameData)
    {
        // Convert the serializable array into 2d
        Hex[,] hexes = new Hex[hexMap.Width, hexMap.Height];
        foreach (Serializable2d<Hex> hexWithIds in gameData.SerializableArray)
        {
            int x = hexWithIds.Dimension1;
            int y = hexWithIds.Dimension2;

            Hex hex = hexWithIds.hex;
            hex.SetHexMap(hexMap);
            
            hexes[x, y] = hex;

            Unit unit = hex.GetUnit();
            if (unit != null)
            {
                Unit newUnit = (Unit)System.Activator.CreateInstance(
                    unit.GetType(), 
                    new object[]{hexMap});

                newUnit.SetHex(hex);
                newUnit.MovesRemaining = unit.MovesRemaining;
                newUnit.IsEmbarked = unit.IsEmbarked;
                newUnit.FinishedMove = unit.FinishedMove;
                newUnit.Path = unit.Path;

                gameData.Units.Add(newUnit);
            }
        }
        return hexes;
    }
}


