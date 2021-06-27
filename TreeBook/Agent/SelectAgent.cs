using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectAgent : MonoBehaviour
{
    [SerializeField] SecretaryAgent secretary;
    float call_act;
    void Update()
    {
        TouchUpdate();
    }
    void TouchUpdate()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (call_act > 0f) //도우미 호출
            {
                call_act = -1f;
                if (select_ui.activeSelf) select_ui.SetActive(false);
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
                    if (select_ui.activeSelf) select_ui.SetActive(false);
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

    [SerializeField] MyScrollRect select_slot_rect;
    [SerializeField] Sprite book;
    [SerializeField] Sprite error;
    [SerializeField] GameObject select_slot_library;
    [SerializeField] GameObject select_cast;
    [SerializeField] float click_timer;
    float timer;

    void OnEnable()
    {
        foreach (Transform slot in select_slot_library.transform)
        {
            slot.gameObject.SetActive(false);
        }
        secretary.GetEpisodeData(out EpisodeDB.EpisodeDescription ed);
        if (ed == null) return;

        var select_count = ed.Count;
        var slot_count = select_slot_library.transform.childCount;

        if(select_count >= slot_count)
        {
            for(int i = 0, icount = select_count - slot_count+1; i<icount; i++)
            {
                var slot = Instantiate(select_cast, select_slot_library.transform);
                slot.SetActive(false);
            }
        }

        for(int i = 0; i<select_count; i++)
        {
            var slot = select_slot_library.transform.GetChild(i + 1).gameObject.GetComponent<SelectSlot>();
            secretary.GetSelectionData(ed[i], out SelectionDB.SelectionDescription sd);
            if (sd == null) continue;
            bool button = true;
            Sprite image;
            for(int j = 0, jcount = sd.Count; j<jcount; j++)
            {
                if (!secretary.IsEpisode(sd[j]))
                {
                    button = false;
                    break;
                }
            }
            if (button) image = book;
            else image = error;
            slot.Set(image, sd, button);
            slot.gameObject.SetActive(true);
        }
        select_slot_rect.Init();
    }
 
    [SerializeField] GameObject select_ui;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI body;
    [SerializeField] MyScrollRect msr;
    SelectionDB.SelectionDescription sd;
    public void SelectPressed()
    {
        if (count_coroutine != null) StopCoroutine(count_coroutine);
        count_coroutine = Count();
        StartCoroutine(count_coroutine);
    }
    public void SelectRelease(SelectSlot select_slot)
    {
        if (!gameObject.activeSelf) return;
        else if (timer > click_timer) return;
        select_ui.SetActive(true);
        sd = select_slot.SelectionData;
        title.text = sd.Title;
        body.text = sd.Context;
        msr.Init();
    }
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
    public void Back()
    {
        select_ui.gameObject.SetActive(false);
        sd = null;
    }
    public void Select()
    {
        select_ui.gameObject.SetActive(false);
        secretary.CallSecretary();
        secretary.NextScenario(sd);
    }
}
