using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReadyStartScript : MonoBehaviour
{
    // Start is called before the first frame update
    static ReadyStartScript rss;
    [SerializeField] TextMeshProUGUI textMPU; //M<Mesh>P<Pro>U<UGUI>
    [SerializeField] Color ready;
    [SerializeField] Color start;
    void Awake()
    {
        rss = this;
    }

    public static void ReadyFunc()
    {
        rss.textMPU.color = rss.ready;
        rss.textMPU.text = "준비~";
    }
    public static void StartFunc()
    {
        rss.textMPU.color = rss.start;
        rss.textMPU.text = "출발!";
    }
    public static void Hide()
    {
        rss.gameObject.SetActive(false);
    }

    public static void Active()
    {
        rss.gameObject.SetActive(true);
    }
}
