using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PagePenSound : MonoBehaviour
{
    static PagePenSound pps;
    [SerializeField] AudioClip page;
    [SerializeField] AudioClip pen;
    [SerializeField] AudioSource audioItem;
    void Awake()
    {
        pps = this;
    }

    public static void Play(PagePenSoundTrack ppst, bool loop)
    {
        if (!pps)
        {
            Debug.Log("에러 -> PagePenSound는 준비가 아직 안 됐습니다.");
            return;
        }
        switch (ppst)
        {
            case PagePenSoundTrack.PageSound:
                {
                    pps.audioItem.clip = pps.page;
                }break;
            case PagePenSoundTrack.PenSound:
                {
                    pps.audioItem.clip = pps.pen;
                }break;
        }
        pps.audioItem.loop = loop;
        pps.audioItem.Play();
    }
    public static void Stop()
    {
        if (!pps)
        {
            Debug.Log("에러 -> PagePenSound는 준비가 아직 안 됐습니다.");
            return;
        }

        pps.audioItem.Stop();
    }

    public enum PagePenSoundTrack
    {
        PageSound,
        PenSound
    }
}
