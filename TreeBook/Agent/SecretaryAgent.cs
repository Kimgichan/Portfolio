using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class SecretaryAgent : MonoBehaviour
{
    [System.Serializable] class SecretaryJson
    {
        public List<string> senario_keys;
        public List<string> episode_keys;
        public SecretaryJson()
        {
            senario_keys = new List<string>();
            episode_keys = new List<string>();
        }
    }
    HashSet<string> episode_check;
    [SerializeField] SecretaryJson secretary_data;
    [SerializeField] float load_timer;
    [System.Serializable] class Guide
    {
        public string book;
    }
    [SerializeField] Guide guide;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        episode_check = new HashSet<string>();
        call = false;
    }

    private void Start()
    {
        active = false;
        string path = $"{Application.persistentDataPath}/guide.txt";
        guide.book = "";
        if (File.Exists(path))
        {
            using (var r = new StreamReader(path))
            {
                guide = JsonUtility.FromJson<Guide>(r.ReadToEnd());
            }
        }
        if(guide == null) guide = new Guide();

        var directList = new DirectoryInfo(Application.persistentDataPath).GetFiles("*.zip");
        //guide.book의 디렉토리가 있는지 확인, 없으면 book = ""값으로 조정
        if (guide.book != ""){
            bool check = false;
            var file = $"{guide.book}.zip";
            foreach (var book in directList)
            {
                if (file == book.Name)
                {
                    check = true;
                    break;
                }
            }
            if (!check)
            {
                guide.book = "";
            }
        }
        if(guide.book == "")
        {
            title.text = "제목";
            minTitle.text = "부제목";
            active = true;
        }
        else
        {
            if (load_book != null)
                StopCoroutine(load_book);
            load_book = LoadBook();
            StartCoroutine(load_book);
        }
    }

    IEnumerator load_book = null;
    IEnumerator LoadBook()
    {
        //using(var w = new StreamWriter($"{Application.persistentDataPath}/guide.txt"))
        //{
        //    w.Write(JsonUtility.ToJson(guide));
        //}
        ScenarioDB.ImportScenarioDB(guide.book, 30);
        EpisodeDB.ImportEpisodeDB(guide.book, 30);
        SelectionDB.ImportSelectionDB(guide.book, 30);

        float load = load_timer;
        yield return null;
        while (load > 0f)
        {
            load -= Time.deltaTime;
            yield return null;
            if (!ScenarioDB.IfUseDB()) continue;
            if (!EpisodeDB.IfUseDB()) continue;
            if (!SelectionDB.IfUseDB()) continue;
            string path = $"{Application.persistentDataPath}/{guide.book}.txt";
            if (File.Exists(path))
            {
                using(var r = new StreamReader(path))
                {
                    secretary_data = JsonUtility.FromJson<SecretaryJson>(r.ReadToEnd());
                    if (secretary_data == null)
                        secretary_data = new SecretaryJson();
                }
            }
            episode_check.Clear();
            foreach (var e in secretary_data.episode_keys)
            {
                episode_check.Add(e);
            }
            if (secretary_data.senario_keys.Count == 0) secretary_data.senario_keys.Add(ScenarioDB.FirstKey);
            if (secretary_data.episode_keys.Count == 0 && !CreateEpisode())
            {
                title.text = "제목";
                minTitle.text = "부제목";
                active = true;
            }
            else
            {
                Setting();
            }
            yield break;
        }
        title.text = "제목";
        minTitle.text = "부제목";
        active = true;
    }

    public bool change_book = false;
    public void ChangeBook(string book_name)
    {
        if (File.Exists($"{Application.persistentDataPath}/{book_name}.zip"))
        {
            guide.book = book_name;
            active = false;
            if (load_book != null)
                StopCoroutine(load_book);
            load_book = LoadBook();
            StartCoroutine(load_book);
        }
    }

    bool CreateEpisode()
    {
        ScenarioDB.GetScenario(secretary_data.senario_keys[secretary_data.senario_keys.Count - 1],
            out ScenarioDB.ScenarioDescription sd);
        if (sd == null) return false;
        float total = 0f;
        int icount = sd.Count;
        for(int i = 0; i<icount; i++)
        {
            total += sd[i].Percentage;
        }
        var p = Random.Range(0f, total);
        total = 0f;
        string ed_key ="";
        for(int i = 0; i<icount; i++)
        {
            total += sd[i].Percentage;
            if (p <= total)
            {
                ed_key = sd[i].Key;
                break;
            }
        }
        EpisodeDB.GetEpisode(ed_key, out EpisodeDB.EpisodeDescription ed);
        if (ed == null) return false;
        secretary_data.episode_keys.Add(ed_key);
        return true;
    }
    void Setting()
    {
        ScenarioDB.GetScenario(secretary_data.senario_keys[secretary_data.senario_keys.Count - 1],
            out ScenarioDB.ScenarioDescription sd);
        var ed_key = secretary_data.episode_keys[secretary_data.episode_keys.Count - 1];
        EpisodeDB.GetEpisode(ed_key, out EpisodeDB.EpisodeDescription ed);
        if(sd==null || ed == null)
        {
            secretary_data.senario_keys.RemoveAt(secretary_data.senario_keys.Count - 1);
            var epList = secretary_data.episode_keys;
            epList.RemoveAt(epList.Count - 1);
            episode_check.Clear();
            foreach (var e in epList)
                episode_check.Add(e);
            if(epList.Count == 0)
            {
                title.text = "제목";
                minTitle.text = "부제목";
                using(var w = new StreamWriter($"{Application.persistentDataPath}/guide.txt"))
                {
                    w.Write("");
                }
                secretary_data.senario_keys.Clear();
                secretary_data.episode_keys.Clear();
                using (var w = new StreamWriter($"{Application.persistentDataPath}/{guide.book}.txt"))
                {
                    w.Write(JsonUtility.ToJson(secretary_data));
                }
                guide.book = "";
                active = true;
                return;
            }
            Setting();
            return;
        }
        using (var w = new StreamWriter($"{Application.persistentDataPath}/guide.txt"))
        {
            w.Write(JsonUtility.ToJson(guide));
        }
        using (var w = new StreamWriter($"{Application.persistentDataPath}/{guide.book}.txt"))
        {
            w.Write(JsonUtility.ToJson(secretary_data));
        }

        title.text = guide.book;
        minTitle.text = sd.Title;
        eAgent.Init(ed.Context, !episode_check.Contains(ed_key));
        episode_check.Add(ed_key);
        active = true;
    }

    public void NextScenario(SelectionDB.SelectionDescription selectD)
    {
        ScenarioDB.GetScenario(selectD.NextScenarioKey, out ScenarioDB.ScenarioDescription sd);
        if (sd == null) return;
        active = false;
        secretary_data.senario_keys.Add(selectD.NextScenarioKey);
        if (CreateEpisode())
        {
            Setting();
        }
    }

    [SerializeField] Animator animator;
    [SerializeField] EpisodeAgent eAgent;
    [SerializeField] BookSelectedAgent bAgent;
    [SerializeField] SelectAgent sAgent;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI minTitle;
    [SerializeField] float call_secretary;
    public float CallTimer => call_secretary;

    [SerializeField] int stress;
    bool call = true;
    bool active;

    /// <summary> 버튼 이벤트
    public void OnRequest_Select()
    {
        if (!active) return;
        active = false;
        sAgent.gameObject.SetActive(true);
        animator.Play("Close", -1, 0f);
    }
    public void OnRequest_Play()
    {
        if (!active) return;
        active = false;
        eAgent.gameObject.SetActive(true);
        animator.Play("Close", -1, 0f);
    }
    public void OnRequest_Option()
    {
        if (!active) return;
        active = false;
        animator.Play("Close", -1, 0f);
    }
    public void OnRequest_Stop()
    {
        if (!active) return;
        active = false;
        animator.Play("Close", -1, 0f);
    }
    public void OnRequest_Choice()
    {
        if (!active) return;
        active = false;
        bAgent.gameObject.SetActive(true);
        bAgent.Restart();
        animator.Play("Close", -1, 0f);
    }

    public void CallSecretary()
    {
        if (!call) return;
        call = false;
        gameObject.SetActive(true);
        animator.Play("Open", -1, 0f);
        if (change_book)
        {
            ChangeBook(bAgent.change_book);
            change_book = false;
        }
    }
    public bool Call => call;
    public void OpenSecretary()
    {
        active = true;
        eAgent.gameObject.SetActive(false);
        bAgent.gameObject.SetActive(false);
        sAgent.gameObject.SetActive(false);
    }
    public void CloseSecretary()
    {
        call = true;
    }

    public void GetEpisodeData(out EpisodeDB.EpisodeDescription ed)
    {
        EpisodeDB.GetEpisode(secretary_data.episode_keys[secretary_data.episode_keys.Count - 1], out ed);
    }
    public void GetSelectionData(in string key, out SelectionDB.SelectionDescription sd)
    {
        SelectionDB.GetSelection(key, out sd);
    }

    public bool IsEpisode(string episode_key)
    {
        return episode_check.Contains(episode_key);
    }


    /// </summary>

    /// <summary> Animation 클립과 관련된 내용
    public void SoundPage()
    {
        PagePenSound.Play(PagePenSound.PagePenSoundTrack.PageSound, false);
    }
    /// </summary>
}
