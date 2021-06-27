using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManagerScript : MonoBehaviour
{
    static BGMManagerScript bs;
    [SerializeField] AudioSource audio;
    [SerializeField] AudioClip[] clip_list;
    [SerializeField] string[] clip_names;

    Dictionary<string, AudioClip> list;
    // Start is called before the first frame update
    void Awake()
    { 
        list = new Dictionary<string, AudioClip>();
        for(int i = 0; i<clip_list.Length; i++)
        {
            list.Add(clip_names[i], clip_list[i]);
        }
        bs = this;
    }

    public static void SetValume(float percentage)
    {
        bs.audio.volume = percentage * 0.05f;
    }

    public static void Play(string clipN, bool loop) // N<Name>
    {
        if(bs.list.TryGetValue(clipN, out AudioClip value))
        {
            bs.audio.clip = value;
            bs.audio.loop = loop;
            bs.audio.Play();
        }
    }
    public static bool IsPlay()
    {
        return bs.audio.isPlaying;
    }
    public static void Pause()
    {
        bs.audio.Pause();
    }

    public static void Stop()
    {
        bs.audio.Stop();
    }
}
