using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    [SerializeField] float curveWeight; //커브값이 늘어나는 양
    [SerializeField] float curveW_mult; //커브 W<Weight>의 증가배율
    [SerializeField] float maxCurve;
    [SerializeField] float speed;
    [SerializeField] Transform[] tr_list;
    [SerializeField] TrailRenderer trail;
    [SerializeField] Animator anim;
    [SerializeField] SpriteRenderer arrowIcon;
    [SerializeField] float ready_time;

    [HideInInspector] public bool run;

    float curve;
    float curveW;
    int currentInput;
    bool success = false;

    // Start is called before the first frame update
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        curve = 0;
        currentInput = 0;
        curveW = curveWeight;
        StartCoroutine(ReadyStart());
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause && run)
        {
            run = false;
            BGMManagerScript.Pause();
            Timer.TimerStop();
            UI_Manager.ActiveUI(Camera.main, "pause", this);
        }
    }

    public void Replay()
    {
        var spawnObject = GameObject.Find("Spawn");
        if (spawnObject)
        {
            curve = 0;
            transform.SetPositionAndRotation(spawnObject.transform.position, spawnObject.transform.rotation);
            currentInput = 0;
            curveW = curveWeight;
            trail.enabled = true;
            trail.Clear();
            arrowIcon.enabled = true;
            Timer.TimerEnd();
            BGMManagerScript.Stop();
            UI_Manager.InactiveUI();
            StartCoroutine(ReadyStart());
        }
    }

    IEnumerator ReadyStart()
    {
        float timer = ready_time;
        Timer.Show();
        EffectSoundManager.Play("ready", false);
        ReadyStartScript.Active();
        ReadyStartScript.ReadyFunc();
        while (true)
        {
            yield return null;
            timer -= Time.deltaTime;
            if(timer < 0f)
            {
                EffectSoundManager.Play("start", false);
                ReadyStartScript.StartFunc();
                run = true;
                Timer.TimerStart();
                StartCoroutine(PlayBGMStart());
                break;
            }
        }
    }

    IEnumerator PlayBGMStart()
    {
        while (EffectSoundManager.IsPlay())
        {
            yield return null;
        }
        ReadyStartScript.Hide();
        BGMManagerScript.Play("play", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (run)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                run = false;
                BGMManagerScript.Pause();
                Timer.TimerStop();
                UI_Manager.ActiveUI(Camera.main, "pause", this);
            }
            Move();
        }
    }

    public void Continue()
    {
        UI_Manager.InactiveUI();
        StartCoroutine(ReadyStart());
    }

    void Move()
    {
        var input = 0;
        if (Input.GetKey(KeyCode.Mouse0))
        {
            var viewPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            if (viewPos.x < 0.5f)
            {
                if (currentInput == 0) currentInput = 1;
                input = 1;
            }
            else
            {
                if (currentInput == 0) currentInput = -1;
                input = -1;
            }
        }


        if(input == 0)
        {
            if(curve != 0f)
            {
                if (curve > 0f)
                {
                    curve -= curveW;
                    if (curve <= 0f)
                    {
                        curveW = curveWeight;
                        currentInput = 0;
                        curve = 0f;
                    }
                }
                else
                {
                    curve += curveW;
                    if (curve >= 0f)
                    {
                        curveW = curveWeight;
                        currentInput = 0;
                        curve = 0f;
                    }
                }
            }
        }
        else
        {
            if (currentInput != 0 && currentInput != input)
            {
                curveW *= curveW_mult;
                currentInput = input;
            }
            curve += (float)(input) * curveW;
            if (Mathf.Abs(curve) > maxCurve)
            {
                curve = (float)(input) * maxCurve;
            }
        }

        Vector3[] pos_list = new Vector3[3];
        for(int i = 0; i<3; i++)
        {
            pos_list[i] = tr_list[i].position;
        }


        var trR = transform.eulerAngles;
        trR.z += curve * Time.deltaTime;
        transform.rotation = Quaternion.Euler(trR);

        var trP = transform.position;
        trP += transform.up * speed * Time.deltaTime;
        transform.position = trP;


        for (int i = 0; i<3; i++)
        {
            var vec = tr_list[i].position - pos_list[i];
            var hit = Physics2D.Raycast(pos_list[i], vec.normalized, vec.magnitude);
            if (hit)
            {
                if (hit.collider.gameObject.tag == "EndLine")
                {
                    run = false;
                    success = true;
                    trail.enabled = false;
                    BGMManagerScript.Stop();
                    //BGMManagerScript.Play("die", false);                   
                    anim.Play("Boom");
                    Timer.TimerStop();
                }
                else
                {
                    run = false;
                    success = false;
                    trail.enabled = false;
                    BGMManagerScript.Stop();
                    EffectSoundManager.Play("boom", false);
                    anim.Play("Boom");
                    Timer.TimerStop();
                }
                break;
            }
        }
    }

    public void ArrowHide()
    {
        arrowIcon.enabled = false;
    }
    public void MenuActive()
    {
        if (!success)
        {
            BGMManagerScript.Stop();
            BGMManagerScript.Play("late", true);
            Timer.TimerEnd();
            UI_Manager.ActiveUI(Camera.main, "fail", this);
        }
        else
        {
            BGMManagerScript.Stop();
            BGMManagerScript.Play("success", true);
            Timer.TimerStop();
            UI_Manager.ActiveUI(Camera.main, "success", this);
        }
    }
}
