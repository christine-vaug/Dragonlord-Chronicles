using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public static class SerializationManager {



    private static string saveDataLocation = Application.persistentDataPath + "//SaveData//";
    private const string modifiedDataPath = "/StreamingAssets/data.json";


    public static bool LoadDataBinary<T> (string filename, out T data) {



        filename = saveDataLocation + filename;

        if (File.Exists(filename)) {

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filename, FileMode.Open);
            data = (T)bf.Deserialize(file);
            file.Close();

            return true;
        }

        data = default(T);
        return false;
    }

    public static void SaveDataBinary (string filename, object data) {

        filename = saveDataLocation + filename;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(filename);
        bf.Serialize(file, data);
        file.Close();
    }

    public static T[] LoadAllBinary<T> (string directory) {


        List<string> files = GetAllFiles(directory, true);

        T[] data = new T[files.Count];
        int idx = 0;

        foreach (string file in files) {

            LoadDataBinary(file, out data[idx]);
            idx++;
        }

        return data;

    }

    public static bool LoadDataJson<T> (string filename, out T data) {

        
        filename = saveDataLocation + filename;

        if (File.Exists(filename)) {

            string jsonFormat = File.ReadAllText(filename);
            data = JsonUtility.FromJson<T>(jsonFormat);
        }

        data = default(T);
        return false;
    }

    public static T[] LoadAllDataJson<T> (string directory) {

        List<string> files = GetAllFiles(directory, true);

        T[] data = new T[files.Count];
        int idx = 0;

        foreach (string file in files) {

            LoadDataJson(file, out data[idx]);
            idx++;
        }

        return data;
    }

    public static void SaveTextToFile (string[] lines, string filename) {


        Debug.Log(saveDataLocation);
        File.WriteAllLines(saveDataLocation + filename, lines);

    }

    public static bool GetTextFromFile (string filename, out string[] lines) {


        if (File.Exists(saveDataLocation + filename)) {
            lines = File.ReadAllLines(saveDataLocation + filename);
            return true;
        }

        lines = null;
        return false;
    }

    public static void SaveDataJson (object data, string filename) {

        string jsonFormat = JsonUtility.ToJson(data, prettyPrint: true);

        string filePath = saveDataLocation + filename;
        File.WriteAllText(filePath, jsonFormat);
    }

    public static List<string> GetAllFiles (string directory, bool includeSubdirectories) {

        List<string> result = new List<string>();
        Internal_GetAllFiles(directory, includeSubdirectories, result);
        return result;
    }

    private static void Internal_GetAllFiles (string directory, bool deeper, List<string> result) {

        string[] dirs = Directory.GetDirectories(directory);
        for (int i = 0; i < dirs.Length; i++) {
            Internal_GetAllFiles(dirs[i], deeper, result);
        }

        string[] files = Directory.GetFiles(directory);
        for (int i = 0; i < files.Length; i++) {

            if (Path.GetExtension(files[i]) == ".txt") {
                result.Add(files[i]);
            }
        }
    }

    public static void CreateNewSaveFile () {

        if (Directory.Exists(saveDataLocation)) {
            Directory.Delete(saveDataLocation, true);
        }

        Directory.CreateDirectory(saveDataLocation);
    }

    public static bool SaveExists () {

        return Directory.Exists(saveDataLocation) && Directory.GetFiles(saveDataLocation).Length > 0;
    }
}
