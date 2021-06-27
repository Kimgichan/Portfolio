using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScenarioDB : MonoBehaviour
{
    static ScenarioDB sDB;
    Dictionary<string, ScenarioDescription> sDic;
    bool sDB_active = false;
    string first_scenario_key = null;
    public static string FirstKey => sDB.first_scenario_key;
    void Awake()
    {
        if (sDB)
        {
            Destroy(this.gameObject);
            return;
        }
        sDic = new Dictionary<string, ScenarioDescription>();
        sDB = this;
        sDB_active = true;
    }
    [System.Serializable] public class ScenarioDescription
    {
        [SerializeField] string title;
        public string Title => title;
        [SerializeField] List<EpisodeNode> episodeDB;
        public EpisodeNode this[int index]
        {
            get
            {
                return episodeDB[index];
            }
        }
        public int Count => episodeDB.Count;
        public ScenarioDescription(string title, List<EpisodeNode> episodeDB)
        {
            this.title = title;
            this.episodeDB = episodeDB;
        }
    }
    [System.Serializable] public class EpisodeNode
    {
        [SerializeField] string key;
        public string Key => key;
        [SerializeField] float percentage;
        public float Percentage => percentage;
        public EpisodeNode(string key, float percentage)
        {
            this.key = key;
            this.percentage = percentage;
        }
    }

    public static void ImportScenarioDB(string zip_name, int stress)
    {
        if (!sDB)
        {
            Debug.Log("에러 -> 시나리오 DB가 아직 구성되지 못했습니다.");
            return;
        }
#if UNITY_EDITOR
        sDB.scenario_view.Clear();
#endif
        sDB.first_scenario_key = null;
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
            using (var sr = new StreamReader(zip.GetEntry("scenario.txt").Open()))
            {
                int s = -1; //s = stress_hellper;
                int separator = 0; //"[->++ ]->--"
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
                        if (c == '*') summary = !summary;
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
                                    ToSenarioNode(scenarioData);
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
            }
        }
        to_file_coroutine = null;
        sDB_active = true;
#if UNITY_EDITOR
        foreach(var s in sDic)
        {
            scenario_view.Add(new ScenarioInspectorView(s.Key, s.Value));
        }
#endif
    }

    void ToSenarioNode(List<string> scenarioData)
    {
        var eNL = new List<EpisodeNode>();
        for(int i=2, icount = scenarioData.Count; i<icount; ++i)
        {
            eNL.Add(new EpisodeNode(scenarioData[i++], float.Parse(scenarioData[i])));
        }
        var sd = new ScenarioDescription(scenarioData[1], eNL);
        sDic.Add(scenarioData[0], sd);
        if (first_scenario_key == null) first_scenario_key = scenarioData[0];
    }

    public static bool IfUseDB()
    {
        if (!sDB)
        {
            Debug.Log("에러 -> 시나리오 DB가 아직 구성되지 못했습니다.");
            return false;
        }
        return sDB.sDB_active;
    }

    public static void GetScenario(string key, out ScenarioDescription sd)
    {
        if (!sDB || !sDB.sDB_active)
        {
            Debug.Log("에러 -> 시나리오 DB가 아직 구성되지 못했습니다.");
            sd = null;
            return;
        }
        if(!sDB.sDic.TryGetValue(key, out ScenarioDescription value))
        {
            Debug.Log("에러 -> 없는 시나리오를 요구했습니다.");
            sd = null;
            return;
        }
        sd = value;
    }

#if UNITY_EDITOR
    [System.Serializable]class ScenarioInspectorView
    {
        [SerializeField] string key;
        [SerializeField] ScenarioDescription data;
        public ScenarioInspectorView(string key, ScenarioDescription data)
        {
            this.key = key;
            this.data = data;
        }
    }
    [SerializeField] List<ScenarioInspectorView> scenario_view;
#endif
}
