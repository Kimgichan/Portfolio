using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TrackRecord : MonoBehaviour
{
    public void Save(string track_name,TrackRecordNode trn)
    {
        var directory_path = Application.persistentDataPath + "/Record";
        if (!Directory.Exists(directory_path))
        {
            Directory.CreateDirectory(directory_path);
        }

        var track_path = directory_path + "/" + track_name + ".txt";
        var bytes = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(trn));
        var code = System.Convert.ToBase64String(bytes);
        File.WriteAllText(track_path, code);
    }

    public void Load(string track_name, out TrackRecordNode trackRN)
    {
        var directory_path = Application.persistentDataPath + "/Record";
        if (!Directory.Exists(directory_path))
        {
            trackRN = null;
            return;
        }

        var track_path = directory_path + "/" + track_name + ".txt";
        if (!File.Exists(track_path))
        {
            trackRN = null;
            return;
        }

        var code = System.Convert.FromBase64String(File.ReadAllText(track_path));
        trackRN = JsonUtility.FromJson<TrackRecordNode>(System.Text.Encoding.UTF8.GetString(code));
    }
}

[System.Serializable] public class TrackRecordNode
{
    public string[] records;
}
