using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoxScroll : MonoBehaviour
{
    RectTransform rect;
    RectTransform scroll;
    public float stress;
    IEnumerator scroll_coroutine = null;

    bool init = false;
    public void Init(RectTransform rect, RectTransform scroll)
    {
        this.rect = rect;
        this.scroll = scroll;
        init = true;
        scroll_coroutine = ScrollAnimation();
        if (enabled)
            StartCoroutine(scroll_coroutine);
    }

    public void InitEnd()
    {
        init = false;
    }

    private void OnDisable()
    {
        if (scroll_coroutine != null)
        {
            StopCoroutine(scroll_coroutine);
            scroll_coroutine = null;
        }
    }
    private void OnEnable()
    {
        if (scroll_coroutine != null)
        {

            StartCoroutine(scroll_coroutine);
        }
    }
    IEnumerator ScrollAnimation()
    {
        Vector3 pos = Vector3.zero;
        while (init)
        {
            yield return null;
            pos = scroll.localPosition;
            pos.y = scroll.rect.height * 0.5f - rect.rect.height * 0.5f;
            scroll.localPosition = pos;
        }

        float force = 0f;
        float dir = 0f;
        float fric = 0f;

        while (true)
        {
            yield return null;
            if (Input.touchCount == 0)
            {
                Inertia(ref pos, ref force, ref dir, ref fric);
                continue;
            }

            var gettouch = Input.GetTouch(0);
            if (gettouch.phase != TouchPhase.Moved) continue;
            if (scroll.rect.height <= rect.rect.height) continue;

            if ((force = gettouch.deltaPosition.y * 600f / Screen.height) < 0f)
            {
                force *= -1f;
                dir = -1f;
                fric = force * stress;
            }
            else dir = 1f;

            pos = scroll.localPosition;
            pos.y += force * dir;

            var max = rect.rect.height * 0.5f - scroll.rect.height * 0.5f;
            var min = scroll.rect.height * 0.5f - rect.rect.height * 0.5f;

            if (pos.y < max)
            {
                pos.y = max;
                force = 0f;
            }
            else if (pos.y > min)
            {
                pos.y = min;
                force = 0f;
            }

        }
    }
    void Inertia(ref Vector3 pos, ref float force, ref float dir, ref float fric)
    {
        if (force <= 0f) return;
        pos = scroll.localPosition;
        pos.y += force * dir;

        var max = rect.rect.height * 0.5f - scroll.rect.height * 0.5f;
        var min = scroll.rect.height * 0.5f - rect.rect.height * 0.5f;

        if (pos.y < max)
        {
            pos.y = max;
            force = 0f;
        }
        else if (pos.y > min)
        {
            pos.y = min;
            force = 0f;
        }
        scroll.localPosition = pos;
        force -= fric;
    }
}
