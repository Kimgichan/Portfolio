using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SelectSlot : MonoBehaviour
{
    [SerializeField] SelectAgent agent;
    [SerializeField] TextMeshProUGUI title;
    SelectionDB.SelectionDescription sd;
    public SelectionDB.SelectionDescription SelectionData => sd;
    [SerializeField] Image image;
    [SerializeField] EventTrigger eventTrigger;

    public void Set(Sprite sprite, SelectionDB.SelectionDescription sd, bool button)
    {
        image.sprite = sprite;
        this.sd = sd;
        this.title.text = sd.Title;
        if (button)
            eventTrigger.enabled = true;
        else
            eventTrigger.enabled = false;
    }
    public void Press()
    {
        agent.SelectPressed();
    }
    public void Release()
    {
        agent.SelectRelease(this);
    }
}
