using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RouletteScript : MonoBehaviour
{
    static RouletteScript rs;
    [SerializeField] GameObject font;
    [SerializeField] GameObject track_lock;
    [SerializeField] TrackRecord track_record;
    [SerializeField] Texture[] tex_list;
    [SerializeField] string[] trap_names;
    [SerializeField] string[] trap_embargos;// 해금 조건들
    [SerializeField] SpriteRenderer middle, left, right;
    [SerializeField] float speed;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI context;
    [SerializeField] GameObject optionUi;
    [SerializeField] GameObject hintUi;

    int pivot = 0;
    bool run = false;
    GameData data;

    void Start()
    {
        rs = this;
        BGMManagerScript.Play("break", true);
        StartCoroutine(GameDataManageLoading());
    }

    IEnumerator GameDataManageLoading()
    {
        while (true)
        {
            yield return null;
            GameDataManager.GameDataLoad(out rs.data);
            if (rs.data != null)
            {
                SettingList();
                SettingData();
                break;
            }
        }
    }

    public static int TrackCount()
    {
        return rs.trap_names.Length;
    }

    public void LeftRot()
    {
        if (run) return;
        run = true;
        font.SetActive(false);
        StartCoroutine(RotCoroutine_Left());
    }
    public void RightRot()
    {
        if (run) return;
        run = true;
        font.SetActive(false);
        StartCoroutine(RotCoroutine_Right());
    }

    IEnumerator RotCoroutine_Left()
    {
        EffectSoundManager.Play("click", false);
        while (true)
        {
            yield return null;

            var angle = transform.eulerAngles;
            angle.z -= speed * Time.deltaTime;
            if (angle.z < 90f) 
            {
                if ((--pivot) < 0)
                {
                    pivot = tex_list.Length - 1;
                }
                
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
                SettingList();
                SettingData();
                run = false;

                break;
            }
            transform.rotation = Quaternion.Euler(angle);
        }
    }
    IEnumerator RotCoroutine_Right()
    {
        EffectSoundManager.Play("click", false);
        while (true)
        {
            yield return null;

            var angle = transform.eulerAngles;
            angle.z += speed * Time.deltaTime;
            if (angle.z > 270f)
            {

                pivot = (pivot + 1) % tex_list.Length;
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
                SettingList();
                SettingData();
                run = false;
                break;
            }
            transform.rotation = Quaternion.Euler(angle);
        }
    }

    void SettingList()
    {
        middle.material.SetTexture("_Tex", tex_list[pivot]);
        if (data.open_list[pivot]) middle.color = Color.white;
        else middle.color = Color.black;

        right.material.SetTexture("_Tex", tex_list[(pivot + 1) % tex_list.Length]);
        if (data.open_list[(pivot + 1) % tex_list.Length]) right.color = Color.white;
        else right.color = Color.black;

        left.material.SetTexture("_Tex", tex_list[(pivot + 2) % tex_list.Length]);
        if (data.open_list[(pivot + 2) % tex_list.Length]) left.color = Color.white;
        else left.color = Color.black;
    }

    public static void Play()
    {
        if (!rs.track_lock.activeSelf && !rs.optionUi.activeSelf && !rs.hintUi.activeSelf)
        {
            BGMManagerScript.Stop();
            SceneManager.LoadScene(rs.trap_names[rs.pivot]);
        }
    }

    void SettingData()
    {
        title.text = trap_names[pivot];
        if (data.open_list[pivot])
        {
            track_lock.SetActive(false);
            font.SetActive(true);
            track_record.Load(trap_names[pivot], out TrackRecordNode trackRN);
            if (trackRN == null)
            {
                context.text = "1등 00:00.000\n2등 00:00.000\n3등 00:00.000";
                return;
            }
            context.text = "1등 " + trackRN.records[0] + "\n" +
                "2등 " + trackRN.records[1] + "\n" +
                "3등 " + trackRN.records[2];
            return;
        }
        track_lock.SetActive(true);
        context.text = "<해금 조건>\n" + trap_embargos[pivot];
    }
}
