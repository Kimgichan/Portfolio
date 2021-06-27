using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SoundOption : MonoBehaviour
{
    public void Save(SoundRecord sound_record)
    {
        string path = Application.persistentDataPath + "/Option";
        var dir = new DirectoryInfo(path);
        if (!dir.Exists) dir.Create();

        path += "/option.txt";
        var contect = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(sound_record)));
        File.WriteAllText(path, contect);
    }

    public void Load(out SoundRecord sound_record)
    {
        string path = Application.persistentDataPath + "/Option";
        if (!Directory.Exists(path))
        {
            sound_record = null;
            return;
        }

        path += "/option.txt";
        if (!File.Exists(path))
        {
            sound_record = null;
            return;
        }

        var code = System.Convert.FromBase64String(File.ReadAllText(path));
        sound_record = JsonUtility.FromJson<SoundRecord>(System.Text.Encoding.UTF8.GetString(code));
    }
}

[System.Serializable] public class SoundRecord
{
    public int bgm;
    public int effect;
}
