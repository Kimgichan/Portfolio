using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SelectionDB : MonoBehaviour
{
    static SelectionDB sDB;
    Dictionary<string, SelectionDescription> sDic;
    bool sDB_active = false;

    void Awake()
    {
        if (sDB)
        {
            Destroy(this.gameObject);
            return;
        }
        sDic = new Dictionary<string, SelectionDescription>();
        sDB = this;
        sDB_active = true;
    }

    [System.Serializable] public class SelectionDescription
    {
        [SerializeField] string title;
        public string Title => title;

        [SerializeField, Multiline] string context;
        public string Context => context;
        [SerializeField] string scenario_key;
        public string NextScenarioKey => scenario_key;
        [SerializeField] List<string> episode_keys;
        public string this[int index]
        {
            get
            {
                return episode_keys[index];
            }
        }
        public int Count => episode_keys.Count;

        public SelectionDescription(string title, string context, string scenario_key, List<string> episode_keys)
        {
            this.title = title;
            this.context = context;
            this.scenario_key = scenario_key;
            this.episode_keys = episode_keys;
        }
    }
#if UNITY_EDITOR
    [System.Serializable]
    class SelectionInspectorView
    {
        [SerializeField] string key;
        [SerializeField] SelectionDescription data;
        public SelectionInspectorView(string key, SelectionDescription data)
        {
            this.key = key;
            this.data = data;
        }
    }
    [SerializeField] List<SelectionInspectorView> selection_view;
#endif

    public static void ImportSelectionDB(string zip_name, int stress)
    {
        if (!sDB)
        {
            Debug.Log("에러 ->  선택지 DB가 아직 구성되지 못했습니다.");
            return;
        }
#if UNITY_EDITOR
        sDB.selection_view.Clear();
#endif
        sDB.sDB_active = false; //DB 사용 중지. 불러오기 실행
        sDB.sDic.Clear();
        if (sDB.to_file_coroutine != null) sDB.StopCoroutine(sDB.to_file_coroutine);
        sDB.to_file_coroutine = sDB.ToFile(zip_name, stress);
        sDB.StartCoroutine(sDB.to_file_coroutine);
    }

    IEnumerator to_file_coroutine = null;
    IEnumerator ToFile(string zip_name, int stress)
    {
        using (var zip = System.IO.Compression.ZipFile.OpenRead($"{Application.persistentDataPath}/{zip_name}.zip"))
        {
            using (var sr = new StreamReader(zip.GetEntry("selection.txt").Open()))
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

                                        ToSelection(scenarioData);
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
        sDB_active = true;
#if UNITY_EDITOR
        foreach (var s in sDic)
        {
            selection_view.Add(new SelectionInspectorView(s.Key, s.Value));
        }
#endif
    }

    void ToSelection(List<string> scenarioData)
    {
        List<string> episodeList = new List<string>();
        for (int i = 4, icount = scenarioData.Count; i < icount; i++)
        {
            episodeList.Add(scenarioData[i]);
        }
        sDic.Add(scenarioData[0], new SelectionDescription(scenarioData[1], scenarioData[2], scenarioData[3], episodeList));
    }

    public static bool IfUseDB()
    {
        if (!sDB)
        {
            Debug.Log("에러 -> 선택지 DB가 아직 구성되지 못했습니다.");
            return false;
        }
        return sDB.sDB_active;
    }
    public static void GetSelection(string key, out SelectionDescription sd)
    {
        if (!sDB || !sDB.sDB_active)
        {
            Debug.Log("에러 -> 선택지 DB가 아직 구성되지 못했습니다.");
            sd = null;
            return;
        }
        if (!sDB.sDic.TryGetValue(key, out SelectionDescription value))
        {
            Debug.Log("에러 -> 없는 선택지를 요구했습니다.");
            sd = null;
            return;
        }
        sd = value;
    }
}
