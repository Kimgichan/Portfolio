using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EpisodeDB : MonoBehaviour
{
    static EpisodeDB eDB;
    Dictionary<string, EpisodeDescription> eDic;
    bool eDB_active = false;

    void Awake()
    {
        if (eDB)
        {
            Destroy(this.gameObject);
            return;
        }
        eDic = new Dictionary<string, EpisodeDescription>();
        eDB = this;
        eDB_active = true;
    }

    [System.Serializable] public class EpisodeDescription
    {
        [SerializeField, Multiline] string context;
        public string Context => context;
        [SerializeField] List<string> selection_keys;
        public string this[int index]
        {
            get
            {
                return selection_keys[index];
            }
        }
        public int Count => selection_keys.Count;

        public EpisodeDescription(string context, List<string> selection_keys)
        {
            this.context = context;
            this.selection_keys = selection_keys;
        }
    }
#if UNITY_EDITOR
    [System.Serializable]
    class EpisodeInspectorView
    {
        [SerializeField] string key;
        [SerializeField] EpisodeDescription data;
        public EpisodeInspectorView(string key, EpisodeDescription data)
        {
            this.key = key;
            this.data = data;
        }
    }
    [SerializeField] List<EpisodeInspectorView> episode_view;
#endif

    public static void ImportEpisodeDB(string zip_name, int stress)
    {
        if (!eDB)
        {
            Debug.Log("에러 -> 에피소드 DB가 아직 구성되지 못했습니다.");
            return;
        }
#if UNITY_EDITOR
        eDB.episode_view.Clear();
#endif
        eDB.eDB_active = false; //DB 사용 중지. 불러오기 실행
        eDB.eDic.Clear();
        if (eDB.to_file_coroutine != null)
            eDB.StopCoroutine(eDB.to_file_coroutine);
        eDB.to_file_coroutine = eDB.ToFile(zip_name, stress);
        eDB.StartCoroutine(eDB.to_file_coroutine);
    }

    IEnumerator to_file_coroutine = null;
    IEnumerator ToFile(string zip_name, int stress)
    {
        using (var zip = System.IO.Compression.ZipFile.OpenRead($"{Application.persistentDataPath}/{zip_name}.zip"))
        {
            using (var sr = new StreamReader(zip.GetEntry("episode.txt").Open()))
            {
                int s = -1; //s = stress_hellper;
                int separator = 0; //"[->++ ]->--"
                int context = 0; // <를 만나면 ++ >를 만나면 --
                List<string> scenarioData = new List<string>();
                List<char> byteString = new List<char>();
                bool summary = false;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (stress > 0 && (++s) >= stress)
                    {
                        s = -1;
                        yield return null;
                    }

                    foreach (var c in line)
                    {
                        if (c == '<' && separator > 0) ++context;
                        else if (c == '>' && separator > 0) --context;
                        else
                        {
                            if (context > 0) byteString.Add(c);
                            else if (c == '*') summary = !summary;
                            else if (!summary)
                            {
                                if (c == '[') ++separator;
                                else if (c == ']')
                                {
                                    --separator;
                                    if (separator == 1)
                                    {
                                        scenarioData.Add(new string(byteString.ToArray()));
                                        byteString.Clear();
                                    }
                                    else
                                    {
                                        if (byteString.Count > 0)
                                        {
                                            scenarioData.Add(new string(byteString.ToArray()));
                                            byteString.Clear();
                                        }
                                        ToEpisode(scenarioData);
                                        scenarioData.Clear();
                                    }
                                }
                                else
                                {
                                    if (c == '/')
                                    {
                                        if (byteString.Count > 0)
                                        {
                                            scenarioData.Add(new string(byteString.ToArray()));
                                            byteString.Clear();
                                        }
                                    }
                                    else if (separator > 0)
                                    {
                                        byteString.Add(c);
                                    }
                                }
                            }
                        }
                    }
                    if (context > 0) byteString.Add('\n');
                }
            }
        }
        to_file_coroutine = null;
        eDB_active = true;
#if UNITY_EDITOR
        foreach (var e in eDic)
        {
            episode_view.Add(new EpisodeInspectorView(e.Key, e.Value));
        }
#endif
    }

    void ToEpisode(List<string> scenarioData)
    {
        List<string> selectionList = new List<string>();
        for(int i=2, icount = scenarioData.Count; i<icount; i++)
        {
            selectionList.Add(scenarioData[i]);
        }
        eDic.Add(scenarioData[0], new EpisodeDescription(scenarioData[1], selectionList));
    }

    public static bool IfUseDB()
    {
        if (!eDB)
        {
            Debug.Log("에러 -> 에피소드 DB가 아직 구성되지 못했습니다.");
            return false;
        }
        return eDB.eDB_active;
    }

    public static void GetEpisode(string key, out EpisodeDescription ed)
    {
        if (!eDB || !eDB.eDB_active)
        {
            Debug.Log("에러 -> 에피소드 DB가 아직 구성되지 못했습니다.");
            ed = null;
            return;
        }
        if (!eDB.eDic.TryGetValue(key, out EpisodeDescription value))
        {
            Debug.Log("에러 -> 없는 에피소드를 요구했습니다.");
            ed = null;
            return;
        }
        ed = value;
    }
}
