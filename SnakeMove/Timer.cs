using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txt;
    [SerializeField] Canvas canvas;
    static Timer t;
    float timer;
    bool run;

    void Awake()
    {
        t = this;
    }

    public static void TimerStop()
    {
        t.canvas.gameObject.SetActive(false);
        t.run = false;
    }
    public static void TimerEnd()
    {
        t.timer = 0f;
        t.txt.text = t.TimeFormat();
        t.canvas.gameObject.SetActive(false);
    }
    public static void TimerStart()
    {
        t.canvas.gameObject.SetActive(true);
        t.run = true;
    }
    public static void Show()
    {
        t.canvas.gameObject.SetActive(true);
    }
    public static string GetTime()
    {
        return t.TimeFormat();
    }

    // Update is called once per frame
    void Update()
    {
        if (run)
        {
            timer += Time.deltaTime;
            txt.text = TimeFormat();
        }
    }

    string TimeFormat()
    {
        string mm = ((int)(timer / 60f)).ToString();
        if (mm.Length < 2)
        {
            mm = "0" + mm;
        }
        else
        {
            mm = mm.Substring(mm.Length - 2);
        }

        string ss = ((int)(timer % 60f)).ToString();
        if(ss.Length < 2)
        {
            ss = "0" + ss;
        }

        string ms = ((int)((timer - (float)((int)timer)) * 1000f)).ToString();
        var count = 3 - ms.Length;
        for (int i=0; i<count; i++)
        {
            ms = ms + "0";
        }

        return mm + ":" + ss + "." + ms;
    }
}
