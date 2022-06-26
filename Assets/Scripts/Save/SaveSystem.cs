using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveGeo(Hex[,] hexes)
    {
        BinaryFormatter formatter  = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.sav";
        FileStream stream = new FileStream(path, FileMode.Create);

        GeoData geoData = new GeoData(hexes);

        formatter.Serialize(stream, geoData);
        stream.Close();
    }

    public static GeoData LoadGeo()
    {
        string path = Application.persistentDataPath + "/save.sav";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GeoData geoData = formatter.Deserialize(stream) as GeoData;
            stream.Close();

            Debug.Log("Load ok");

            return geoData;
        }
        else
        {
            Debug.Log("Save file not found in " + path);
            return null;
        }
    }
}
