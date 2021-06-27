using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameDataManager : MonoBehaviour
{
    static GameDataManager gdm;
    void Start()
    {
        gdm = this;
    }

    public static void GameDataLoad(out GameData data)
    {
        if(gdm == null)
        {
            data = null;
            return;
        }

        gdm.Load(out data);
        if (data == null || data.open_list.Length != RouletteScript.TrackCount())
        {
            data = new GameData();
            data.open_list = new bool[RouletteScript.TrackCount()];
            for(int i = 0; i<data.open_list.Length; i++)
            {
                data.open_list[i] = false;
            }
            data.open_list[0] = true;
            gdm.Save(data);
            return;
        }
    }

    public static void ComplateTrack(int key)
    {
        if (gdm == null) return;
        gdm.Load(out GameData data);
        if (data == null) return;

        data.open_list[key] = true;
        gdm.Save(data);
    }

    void Save(GameData data)
    {
        var path = Application.persistentDataPath + "/Game";
        var dir = new DirectoryInfo(path);
        if (!dir.Exists)
        {
            dir.Create();
        }

        path = path + "/gameinfo.txt";
        File.WriteAllText(path, System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(data))));
    }

    void Load(out GameData data)
    {
        var path = Application.persistentDataPath + "/Game";
        var dir = new DirectoryInfo(path);
        if (!dir.Exists)
        {
            data = null;
            return;
        }

        path = path + "/gameinfo.txt";

        if (!File.Exists(path))
        {
            data = null;
            return;
        }

        data = JsonUtility.FromJson<GameData>(System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(File.ReadAllText(path))));
    }
}

[System.Serializable] public class GameData
{
    public bool[] open_list;
}
