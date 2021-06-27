using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class BookSelectedAgent : MonoBehaviour
{
    [SerializeField] SecretaryAgent secretary;
    [SerializeField] BoxScroll bScroll;
    [SerializeField] RectTransform lRect;
    [SerializeField] RectTransform lScroll;

    float call_act = -1f;
    bool close;
    void Update()
    {
        if (close) return;
        TouchUpdate();
    }

    public void Restart()
    {
        close = false;
        web_ui = false;
        var di = new System.IO.DirectoryInfo(Application.persistentDataPath);
        var diList = di.GetFiles("*.zip");
        var diListN = diList.Length;
        var slotN = library.transform.childCount;
        if (diListN >= slotN)
        {
            for (int i = 0, icount = diListN - slotN + 1; i < icount; i++)
                Instantiate(book_cast.gameObject, library.transform);
        }
        else
        {
            for (int i = diListN; i < slotN; i++)
                library.transform.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < diListN; i++)
        {
            var book = library.transform.GetChild(i + 1).gameObject.GetComponent<Book>();
            book.gameObject.SetActive(true);
            book.SetData((diList[i].Name.Split(".txt".ToCharArray()))[0]);
        }
        bScroll.Init(lRect, lScroll);
        bScroll.InitEnd();
    }
    public void Stop()
    {
        if (!secretary.Call) return;
        timer = 0f;
        close = true;
        web_ui = false;
        web_libraryUI.SetActive(false);
        if (BookUI.activeSelf)
            animator.Play("Close", -1, 0f);
        else secretary.CallSecretary();
    }

    void TouchUpdate()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (call_act > 0f) //도우미 호출
            {
                call_act = -1f;
                Stop();
            }
            else
            {
                call_act = secretary.CallTimer;
                StartCoroutine(TouchTimer());
            }
        }
#endif
        if (Input.touchCount > 0)
        {
            var gettouch = Input.GetTouch(0);
            if (gettouch.phase == TouchPhase.Began)
            {
                if (call_act > 0f) //도우미 호출
                {
                    call_act = -1f;
                    Stop();
                }
                else
                {
                    call_act = secretary.CallTimer;
                    StartCoroutine(TouchTimer());
                }
            }
        }
    }

   IEnumerator TouchTimer()
    {
        while (call_act > 0f)
        {
            yield return null;
            call_act -= Time.deltaTime;
        }
        call_act = -1f;
    }



    [SerializeField] GameObject library;
    [SerializeField] GameObject book_cast;
    [SerializeField] float click_timer;
    [SerializeField] Animator animator;
    string book;

    public void SelectPressed(string book)
    {
        if (BookUI.activeSelf) return;
        this.book = book;
        if (count_coroutine != null) StopCoroutine(count_coroutine);
        count_coroutine = Count();
        StartCoroutine(count_coroutine);
    }
    public void SelectPressed(BookUrl book_url)
    {
        if (count_coroutine != null) StopCoroutine(count_coroutine);
        count_coroutine = Count();
        StartCoroutine(count_coroutine);
    }

    float timer;
    IEnumerator count_coroutine = null;
    IEnumerator Count()
    {
        timer = 0f;
        while (true)
        {
            yield return null;
            timer += Time.deltaTime;
            if (timer > click_timer)
            {
                count_coroutine = null;
                break;
            }
        }
    }

    public void SafeCall()
    {
        if (close) return;
        if (click_timer > timer)
        {
            BookUI.gameObject.SetActive(true);
            title.text = book;
            writer.text = "불명";
            var path = $"{Application.persistentDataPath}/{book}.zip";
            if (System.IO.File.Exists(path))
            {
                using(var zip = System.IO.Compression.ZipFile.OpenRead(path))
                {
                    using(var reader = new System.IO.StreamReader(zip.GetEntry("summary.txt").Open()))
                    {
                        var str = reader.ReadLine();
                        if (str != null)
                        {
                            writer.text = str;
                            string line = "";
                            while ((str = reader.ReadLine()) != null)
                            {
                                line += str;
                                line += "\n";
                            }
                            book_ui_writer.StartWrite(line, false);
                            book_ui_textSizeFileter.OpenFilter(line.Length);
                        }
                    }
                }
            }
            animator.Play("Open", -1, 0f);
        }
    }
    public void SafeTimer(string name, string url)
    {
        if (close) return;
        if (click_timer > timer)
        {
            if (web_bookDown_coroutine != null) StopCoroutine(web_bookDown_coroutine);
            web_bookDown_coroutine = WebBookDownload(name, url);
            StartCoroutine(web_bookDown_coroutine);
        }
    }
    IEnumerator web_bookDown_coroutine = null;
    IEnumerator WebBookDownload(string name, string url)
    {
        web_ui = false;
        web_libraryUI.SetActive(false);
        var www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.error == null && www.responseCode <= 200 && !www.downloadHandler.text.Contains("ERROR"))
        {
            string path = $"{Application.persistentDataPath}/{name}.zip";
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            System.IO.File.WriteAllBytes(path, www.downloadHandler.data);
            Restart();
        }
    }

    [SerializeField] GameObject BookUI;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI writer;
    [SerializeField] Writer book_ui_writer;
    [SerializeField] TextSizeFilter book_ui_textSizeFileter;
    public void BookReturn()
    {
        animator.Play("Close", -1, 0f);
    }
    public string change_book;
    public void BookGet()
    {
        secretary.change_book = true;
        change_book = this.title.text;
        Stop();
    }
    public void BookUIOpen()
    {
        PagePenSound.Play(PagePenSound.PagePenSoundTrack.PageSound, false);
    }
    public void BookUIClose()
    {
        PagePenSound.Play(PagePenSound.PagePenSoundTrack.PageSound, false);
    }
    public void BookUICloseEnd()
    {
        BookUI.gameObject.SetActive(false);
        if (close)
            secretary.CallSecretary();
        else if(web_ui)
        {
            web_libraryUI.SetActive(true);
        }
    }


    [SerializeField] GameObject web_libraryUI;
    [SerializeField] GameObject book_list;
    [SerializeField] RectTransform web_rect;
    [SerializeField] RectTransform web_scroll;
    [SerializeField] GameObject web_book_cast;
    bool web_ui = false;

    public void WebLibraryOpen()
    {
        web_ui = true;
        if (BookUI.activeSelf)
        {
            animator.Play("Close", -1, 0f);
        }
        else web_libraryUI.SetActive(true);
        StartCoroutine(WebLibrary());
        bScroll.Init(web_rect, web_scroll);
    }
    public void WebLibraryClose()
    {
        web_ui = false;
        web_libraryUI.SetActive(false);
        bScroll.Init(lRect, lScroll);
    }

    class WebBookData
    {
        public string book_name;
        public string book_url;
        public WebBookData(string name, string url)
        {
            this.book_name = name;
            this.book_url = url;
        }
    }
    List<WebBookData> web_book_list = new List<WebBookData>();
    IEnumerator WebLibrary()
    {
        foreach(Transform o in book_list.transform)
        {
            o.gameObject.SetActive(false);
        }
        web_book_list.Clear();
        UnityWebRequest www = UnityWebRequest.Get(
            "https://docs.google.com/spreadsheets/d/1VhxfdQMMn6bIiTbqqbkiQV6s440Lxf7x5qtbSx2XIM8/export?format=tsv");
        yield return www.SendWebRequest();
        if (www.error == null && www.responseCode <= 200 && !www.downloadHandler.text.Contains("ERROR"))
        {
            string data = www.downloadHandler.text;
            var webBookList = data.Split('\n');
            foreach (var webBook in webBookList)
            {
                var webdata = webBook.Split('\t');
                web_book_list.Add(new WebBookData(webdata[0], webdata[1]));
            }

            var slotN = book_list.transform.childCount;
            var bookListN = web_book_list.Count;

            if (bookListN >= slotN)
            {
                for (int i = 0, icount = bookListN - slotN + 1; i < icount; i++)
                    Instantiate(web_book_cast.gameObject, book_list.transform);
            }
            else
            {
                for (int i = bookListN; i < slotN; i++)
                    book_list.transform.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 0; i < bookListN; i++)
            {
                var book_url = book_list.transform.GetChild(i + 1).gameObject.GetComponent<BookUrl>();
                book_url.Init(web_book_list[i].book_name, web_book_list[i].book_url);
                book_url.gameObject.SetActive(true);
            }
            yield return null;
        }
        bScroll.InitEnd();
    }
}
