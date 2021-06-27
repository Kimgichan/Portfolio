using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextSizeFilter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] RectTransform rect;
    [HideInInspector] public bool auto = false;
    public float filter_time;
    public float stress;
    int order_count;
    IEnumerator write_anim_coroutine = null;

    public void OpenFilter(int max_count)
    {
        order_count = max_count;
        rt = transform as RectTransform;
        write_anim_coroutine = WriterAnimation();
        if (gameObject.activeInHierarchy)
            StartCoroutine(write_anim_coroutine);
    }
    void OnEnable()
    {
        if(write_anim_coroutine != null)
            StartCoroutine(write_anim_coroutine);
    }
    void OnDisable()
    {
        if (write_anim_coroutine != null)
        {
            StopCoroutine(write_anim_coroutine);
            write_anim_coroutine = null;
        }
    }

    float force = 0f;
    float dir;
    float fric;
    RectTransform rt;
    void Update()
    {
        if (Input.touchCount > 0)
        {
            var gettouch = Input.GetTouch(0);
            if (gettouch.phase == TouchPhase.Moved)
            {
                if (rt.rect.height <= rect.rect.height)
                    return;
                if ((force = gettouch.deltaPosition.y * 600f / Screen.height) < 0f)
                {
                    force *= -1f;
                    dir = -1f;
                    fric = force * stress;
                }
                else dir = 1f;
                var pos = rt.localPosition;
                pos.y += force * dir;

                var max = rect.rect.height * 0.5f - rt.rect.height * 0.5f;
                var min = rt.rect.height * 0.5f - rect.rect.height * 0.5f;

                auto = false;
                if (pos.y < max)
                {
                    pos.y = max;
                    force = 0f;
                }
                else if (pos.y > min)
                {
                    pos.y = min;
                    auto = true;
                    force = 0f;
                }
                rt.localPosition = pos;
            }
        }
        else Inertia();
    }

    void Inertia()
    {
        if (force <= 0f)
            return;
        var pos = rt.localPosition;
        pos.y += force * dir;
        var max = rect.rect.height * 0.5f - rt.rect.height * 0.5f;
        var min = rt.rect.height * 0.5f - rect.rect.height * 0.5f;
        if (pos.y < max)
        {
            pos.y = max;
            force = 0f;
        }
        else if (pos.y > min)
        {
            pos.y = min;
            auto = true;
            force = 0f;
        }
        rt.localPosition = pos;
        force -= fric;
    }

    IEnumerator WriterAnimation()
    {
        yield return null;
        auto = true;
        float timer = 0f;
        Vector3 pos;
        
        while (true)
        {
            yield return null;

            timer += Time.deltaTime;
            if (timer > filter_time)
            {
                timer = 0f;

                //사이즈 조정
                var deltaHeight = text.preferredHeight - rt.sizeDelta.y;
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, text.preferredHeight);

                if (auto) //text 위치 조정
                {
                    pos = rt.localPosition;
                    if (rect.rect.height > rt.rect.height)
                        pos.y = rect.rect.height * 0.5f - rt.rect.height * 0.5f;
                    else
                        pos.y = rt.rect.height * 0.5f - rect.rect.height * 0.5f;
                    rt.localPosition = pos;
                }
                else
                {
                    pos = rt.localPosition;
                    pos.y = pos.y - deltaHeight * 0.5f;
                    rt.localPosition = pos;
                }
                if (text.text.Length == order_count)
                {
                    write_anim_coroutine = null;
                    break;
                }
            }

        }
    }
}
