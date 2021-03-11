using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveLoad
{
    public static string directory = "/SaveData/";
    public static string fileName1 = "ChData.txt";
    public static string fileName2 = "ItData.txt";
    public static string fileName3 = "PrData.txt";

    public static void Save() 
    {
        string dir = Application.persistentDataPath + directory;

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string json1 = JsonUtility.ToJson(GameMaster.GM.characters);
        string json2 = JsonUtility.ToJson(GameMaster.GM.items);
        string json3 = JsonUtility.ToJson(GameMaster.GM.progress);

        File.WriteAllText(dir + fileName1, json1);
        File.WriteAllText(dir + fileName2, json2);
        File.WriteAllText(dir + fileName3, json3);

    }
    public static void Load()
    {
        string path1 = Application.persistentDataPath + directory + fileName1;
        string path2 = Application.persistentDataPath + directory + fileName2;
        string path3 = Application.persistentDataPath + directory + fileName3;

        if (File.Exists(path1) && File.Exists(path2) && File.Exists(path3))
        {
            string json1 = File.ReadAllText(path1);
            string json2 = File.ReadAllText(path2);
            string json3 = File.ReadAllText(path3);

            GameMaster.GM.characters = JsonUtility.FromJson<CharacterData>(json1);
            GameMaster.GM.items = JsonUtility.FromJson<ItemData>(json2);
            GameMaster.GM.progress = JsonUtility.FromJson<ProgressData>(json3);
            
        }
    }


}
