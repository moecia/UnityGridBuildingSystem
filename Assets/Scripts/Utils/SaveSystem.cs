using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem
{
    public static void SaveFile(string filename, string saveData)
    {
        var savePath = Path.Combine(Application.persistentDataPath, $"{filename}.json");
        var fileStream = new FileStream(savePath, FileMode.Create);

        using (var writer = new StreamWriter(fileStream))
        {
            writer.Write(saveData);
        }
    }

    public static string LoadFile(string filename)
    {
        var savePath = Path.Combine(Application.persistentDataPath, $"{filename}.json");
        if (File.Exists(savePath))
        {
            using(var reader = new StreamReader(savePath))
            {
                return reader.ReadToEnd();
            }
        }
        else
        {
            Debug.LogError("Cannot find the save file!");
        }
        return null;
    }

    public static void SaveObject(string filename, object saveData)
    {
        string json = JsonConvert.SerializeObject(saveData);
        SaveFile(filename, json);
        Debug.Log("Save success!");
    }

    public static TSaveObject LoadSavedObject<TSaveObject>(string filename)
    {
        var saveData = LoadFile(filename);
        if(saveData != null)
        {
            var saveObject = JsonConvert.DeserializeObject<TSaveObject>(saveData);
            return saveObject;
        }
        return default;
    }
}
