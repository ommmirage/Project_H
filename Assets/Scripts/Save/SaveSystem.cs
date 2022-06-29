using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveGeo(Hex[,] hexes)
    {
        BinaryFormatter formatter  = new BinaryFormatter();

        string path = Application.persistentDataPath + "/save.sav";
        FileStream stream = new FileStream(path, FileMode.OpenOrCreate);

        // GeoData geoData = new GeoData(hexes);

        TestHex[] data = new TestHex[2];
        data[0] = new TestHex(86, null);
        data[1] = new TestHex(1, data[0]);

        formatter.Serialize(stream, hexes);
        stream.Close();

        Debug.Log("Game Saved");
    }

    public static GeoData LoadGeo()
    {
        string path = Application.persistentDataPath + "/save.sav";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            // TestHex[] data = formatter.Deserialize(stream) as TestHex[];
            // GeoData geoData = formatter.Deserialize(stream) as GeoData;
            Hex[,] hexes = formatter.Deserialize(stream) as Hex[,];
            stream.Close();

            Debug.Log(hexes[1,1]);

            Debug.Log("Load ok");

            return null;
            // return geoData;
        }
        else
        {
            Debug.Log("Save file not found in " + path);
            return null;
        }
    }
}
