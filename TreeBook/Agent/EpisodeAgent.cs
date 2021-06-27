using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpisodeAgent : MonoBehaviour
{
    [SerializeField] SecretaryAgent secretary;
    [SerializeField] Writer writer;
    [SerializeField] TextSizeFilter filter;


    float call_act = -1f;

    void Update()
    {
        TouchUpdate();
    }

    public void Init(in string context, bool write)
    {
        writer.StartWrite(in context, write);
        filter.OpenFilter(context.Length);
    }

    void TouchUpdate()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (call_act > 0f) //도우미 호출
            {
                call_act = -1f;
                secretary.CallSecretary();
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
                    secretary.CallSecretary();
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
}
