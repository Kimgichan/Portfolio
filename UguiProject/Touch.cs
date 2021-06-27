using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TSOHandle = TouchSprayObjectHandle;
using EventTabSub = TouchSprayObjectHandle.EventTabSup;
public class Touch : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TSOHandle tsoHandle;
    [SerializeField] Canvas canvas;
    public static Canvas mainCanvas
    {
        get
        {
            return touch.canvas;
        }
    }
    GraphicRaycaster uiRaycaster;
    PointerEventData padPoint;


    static Touch touch;
    bool isTouch = false;

    private void Awake()
    {
        touch = this;
        uiRaycaster = canvas.GetComponent<GraphicRaycaster>();
        padPoint = new PointerEventData(null);
    }

    void Start()
    {
    }

    public static void EventStop()
    {
        touch.tsoHandle.EventStop();
    }
    public static void PositionEventActive()
    {
        EventTabSub.PositionEventEnter(touch.tsoHandle);
    }
    public static void RotationEventActive()
    {
        EventTabSub.RotationEventEnter(touch.tsoHandle);
    }
    public static void ScaleEventActive()
    {
        EventTabSub.ScaleEventEnter(touch.tsoHandle);
    }
    public static void CloseEventActive()
    {
        EventTabSub.CloseEventEnter(touch.tsoHandle);
    }
    public static void FlipX_EventActive()
    {
        EventTabSub.FlipX_EventEnter(touch.tsoHandle);
    }
    public static void RiseEventActive()
    {
        EventTabSub.RiseEventEnter(touch.tsoHandle);
    }

    public static void LinkSprayObject(SprayObject target)
    {
        if (!touch.tsoHandle.EqualTarget(target)) 
        {
            touch.tsoHandle.Deselected();
            touch.tsoHandle.Selected(target);
        }
    }

    void Update()
    {
        //if (Input.touchCount > 0)
        //{
        //    var pos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        //    pos.z = 1f;
        //    img.transform.position = pos;
        //    var rotAngle = img.transform.rotation.eulerAngles;
        //    rotAngle.z += 20f * Time.smoothDeltaTime;
        //    img.transform.rotation = Quaternion.Euler(rotAngle);
        //}
        //if (Input.GetMouseButton(0))
        //{
        //    var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    pos.z = 1f;
        //    img.transform.position = pos;
        //    var rotAngle = img.transform.rotation.eulerAngles;
        //    rotAngle.z += 20f * Time.smoothDeltaTime;
        //    img.transform.rotation = Quaternion.Euler(rotAngle);
        //}
        if (Input.GetMouseButton(0))
        {
            if (!isTouch)
            {
                isTouch = true;
                padPoint.position = Input.mousePosition;
                var results = new List<RaycastResult>();
                uiRaycaster.Raycast(padPoint, results);
                if (results.Count>0)
                {
                    foreach(var result in results)
                    {
                        var tab = result.gameObject.GetComponent<SprayTabObject>();
                        if (tab)
                        {
                            tab.OnClick();
                            return;
                        }
                    }
                    var sObject = results[0].gameObject.GetComponent<SprayObject>();
                    if (sObject)
                    {
                        sObject.OnClick();
                    }
                }
                else
                {
                    tsoHandle.Deselected();
                }
            }
        }
        else
        {
            if (isTouch)
            {
                isTouch = false;
                tsoHandle.EventStop();
            }
        }
    }
}
