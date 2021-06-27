using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject[] panel_list;
    [SerializeField] string[] panel_names;


    static UI_Manager ui_manager;
    Dictionary<string, GameObject> panelD;
    ArrowScript arrow;

    void Start()
    {
        gameObject.SetActive(false);

        panelD = new Dictionary<string, GameObject>();
        for(int i = 0; i<panel_list.Length; i++)
        {
            panelD.Add(panel_names[i], panel_list[i]);
        }

        ui_manager = this;
    }

    public static void ActiveUI(Camera cam, string panel_name, ArrowScript arrow)
    {
        if (!cam || !arrow || !ui_manager.panelD.TryGetValue(panel_name, out GameObject value)) return;

        ui_manager.canvas.worldCamera = cam;
        ui_manager.canvas.gameObject.SetActive(true);
        ui_manager.arrow = arrow;

        foreach(var panel in ui_manager.panelD)
        {
            panel.Value.SetActive(false);
        }

        value.SetActive(true);
        switch (panel_name)
        {
            case "success":
                {
                    ui_manager.SuccessSetting();
                }break;
        }

    }

    void SuccessSetting()
    {
        var rankingTxt = transform.GetChild(2).GetChild(3).GetComponent<TextMeshProUGUI>();
        var rank = GameRecordManager.Renewal();

        if(rank == 0)
        {
            Debug.Log("레코드 쪽 에러");
        }
        else if(rank < 4)
        {
            rankingTxt.text = rank.ToString() + "등";
        }
        else
        {
            rankingTxt.text = "탈락";
        }

        var timeTxt = transform.GetChild(2).GetChild(2).GetComponent<TextMeshProUGUI>();
        timeTxt.text = Timer.GetTime();
    }

    public static void InactiveUI()
    {
        ui_manager.canvas.gameObject.SetActive(false);
        ui_manager.canvas.worldCamera = null;
    }

    public void Restart()
    {
        arrow.Replay();
    }

    public void Continue()
    {
        arrow.Continue();
    }

    public void Back()
    {
        SceneManager.LoadScene("Main");
    }
}
