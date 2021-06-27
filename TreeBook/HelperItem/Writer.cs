using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Writer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    string context; 
    public float write_time;
    IEnumerator write_coroutine = null;

    public void StartWrite(in string context, bool write)
    {
        this.context = context;
        this.write = write;
        if (write)
        {
            write_coroutine = Write();
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(write_coroutine);
            }
        }
        else
        {
            //여기가 에러 부분
            if (gameObject.activeInHierarchy)
            {
                this.write = true;
                text.text = context;
            }
            else this.context = context;
        }
    }
    bool write;

    void OnEnable()
    {
        if(!write)
        {
            this.text.text = context;
            this.write = true;
        }
        else if (write_coroutine != null)
            StartCoroutine(write_coroutine);
    }
    void OnDisable()
    {
        if (write_coroutine != null)
        {
            StopCoroutine(write_coroutine);
            write_coroutine = null;
        }
    }

    IEnumerator Write()
    {
        yield return null;
        float timer = 0f;
        int i = 0, icount = context.Length;
        text.text = "";
        while(true)
        {
            yield return null;
            timer += Time.deltaTime;
            if(timer > write_time)
            {
                timer = 0f;
                if (context[i] != '\n' && context[i] != ' ')
                    PagePenSound.Play(PagePenSound.PagePenSoundTrack.PenSound, false);
                text.text += context[i++].ToString();
                if (i == icount)
                {
                    write_coroutine = null;
                    break;
                }
            }
        }
    }
}
