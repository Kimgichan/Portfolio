using System;
using UnityEngine;

public class MenuHandle : MonoBehaviour
{
    [SerializeField] RectTransform menuUI;
    [SerializeField] RectTransform menuHandle;
    [SerializeField] float openPosY;
    [SerializeField] float closePosY;
    [SerializeField] float speed;
    bool dir = true;
    Action eventAction;
    
    // Start is called before the first frame update
    void Start()
    {
        eventAction = new Action(StopFunc);
        openPosY *= DB_Manager.dbManager.ScreenScaleLerp.y;
        closePosY *= DB_Manager.dbManager.ScreenScaleLerp.y;
        speed *= DB_Manager.dbManager.ScreenScaleLerp.y;
    }

    void StopFunc()
    {
        
    }
    void RiseFunc()
    {
        var pos = menuUI.localPosition;
        pos.y += speed * Time.smoothDeltaTime;
        if (pos.y > openPosY)
        {
            pos.y = openPosY;
            dir = true;
            menuHandle.localRotation = Quaternion.Euler(0f, 0f, 180f);
            eventAction = StopFunc;
        }
        menuUI.localPosition = pos;
    }
    void DecreaseFunc()
    {
        var pos = menuUI.localPosition;
        pos.y -= speed * Time.smoothDeltaTime;
        if (pos.y < closePosY)
        {
            pos.y = closePosY;
            dir = false;
            menuHandle.localRotation = Quaternion.Euler(0f, 0f, 0f);
            eventAction = StopFunc;
        }
        menuUI.localPosition = pos;
    }

    // Update is called once per frame
    void Update()
    {
        eventAction();
    }

    public void OnClick()
    {
        if (dir)
        {
            eventAction = DecreaseFunc;
        }
        else
        {
            eventAction = RiseFunc;
        }
    }
}
