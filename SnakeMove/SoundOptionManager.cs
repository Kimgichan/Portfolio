using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOptionManager : MonoBehaviour
{
    [SerializeField] GameObject optionUI;
    [SerializeField] SoundOption sound_option;
    [SerializeField] int max;
    [SerializeField] GameObject[] bgm_volume;
    [SerializeField] GameObject[] effect_volume;

    static SoundOptionManager som;

    int bgm = 0;
    int effect = 0;

    void Start()
    {
        bgm = max;
        effect = max;
        sound_option.Load(out SoundRecord sound_record);
        if(sound_record != null)
        {
            bgm = sound_record.bgm;
            effect = sound_record.effect;
        }
        else
        {
            var sr = new SoundRecord();
            sr.bgm = bgm;
            sr.effect = effect;
            sound_option.Save(sr);
        }
        foreach (var bgmO in bgm_volume)
        {
            bgmO.SetActive(false);
        }
        foreach (var effectO in effect_volume)
        {
            effectO.SetActive(false);
        }
        if (optionUI)
            optionUI.SetActive(false);


        EffectSoundManager.SetValume((float)effect / (float)max);
        BGMManagerScript.SetValume((float)bgm / (float)max);
        som = this;
    }

    public void OptionUI_Open()
    {
        optionUI.SetActive(true);
        var m = bgm + 1;
        for(int i = 1; i<m; i++)
        {
            bgm_volume[i - 1].SetActive(true);
        }

        m = effect + 1;
        for (int i = 1; i < m; i++)
        {
            effect_volume[i - 1].SetActive(true);
        }
    }
    public void OptionUI_Close()
    {
        foreach ( var bgmO in bgm_volume)
        {
            bgmO.SetActive(false);
        }
        foreach(var effectO in effect_volume)
        {
            effectO.SetActive(false);
        }
        optionUI.SetActive(false);
    }

    public void BgmRiseClick()
    {
        if ((++bgm) > max) bgm = max;
        bgm_volume[bgm - 1].SetActive(true);
        var ro = new SoundRecord();
        ro.bgm = bgm;
        ro.effect = effect;
        sound_option.Save(ro);
        BGMManagerScript.SetValume((float)bgm / (float)max);
    }

    public void BgmDeClineClick()
    {
        if ((--bgm) < 0) bgm = 0;
        else
        {
            bgm_volume[bgm].SetActive(false);
        }
        var ro = new SoundRecord();
        ro.bgm = bgm;
        ro.effect = effect;
        sound_option.Save(ro);
        BGMManagerScript.SetValume((float)bgm / (float)max);
    }

    public void EffectRiseClick()
    {
        EffectSoundManager.Play("click", false);
        if ((++effect) > max) effect = max;
        effect_volume[effect - 1].SetActive(true);
        var ro = new SoundRecord();
        ro.bgm = bgm;
        ro.effect = effect;
        sound_option.Save(ro);

        EffectSoundManager.SetValume((float)effect / (float)max);
    }

    public void EffectDeClineClick()
    {
        EffectSoundManager.Play("click", false);
        if ((--effect) < 0) effect = 0;
        else
        {
            effect_volume[effect].SetActive(false);
        }
        var ro = new SoundRecord();
        ro.bgm = bgm;
        ro.effect = effect;
        sound_option.Save(ro);

        EffectSoundManager.SetValume((float)effect / (float)max);
    }
}
