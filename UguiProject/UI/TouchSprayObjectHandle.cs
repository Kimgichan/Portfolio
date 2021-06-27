using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

using TSOHandle = TouchSprayObjectHandle;
public class TouchSprayObjectHandle : MonoBehaviour
{
    [SerializeField] RectTransform positionTab;
    [SerializeField] RectTransform rotationTab;
    [SerializeField] RectTransform scaleTab;
    [SerializeField] RectTransform closeTab;
    [SerializeField] RectTransform riseTab;
    [SerializeField] RectTransform flipX_tab;
    [SerializeField] float gap;

    SprayObject target;
    Action eventRun;

    void Start()
    {
        target = null;
        eventRun = new Action(Stop);
        gameObject.SetActive(false);
        float lerpUnit = DB_Manager.dbManager.ScreenScaleLerp.x;
        positionTab.sizeDelta *= lerpUnit;
        rotationTab.sizeDelta *= lerpUnit;
        scaleTab.sizeDelta *= lerpUnit;
        closeTab.sizeDelta *= lerpUnit;
        riseTab.sizeDelta *= lerpUnit;
        flipX_tab.sizeDelta *= lerpUnit;
        gap *= lerpUnit;
    }
    void Update()
    {
        eventRun();
    }

    void UiArrage()
    {
        var halfSize = (transform as RectTransform).sizeDelta*0.5f;
        var _gap = positionTab.sizeDelta.x*0.5f;
        positionTab.localPosition = new Vector3(-halfSize.x + _gap, halfSize.y - _gap, 0f);
        rotationTab.localPosition = new Vector3(halfSize.x - _gap, halfSize.y - _gap, 0f);
        scaleTab.localPosition = new Vector3(-halfSize.x + _gap, -halfSize.y + _gap, 0f);
        closeTab.localPosition = new Vector3(halfSize.x - _gap, -halfSize.y + _gap, 0f);
        flipX_tab.localPosition = new Vector3(halfSize.x - _gap, 0f, 0f);
        riseTab.localPosition = new Vector3(-halfSize.x + _gap, 0f, 0f);
    }

    public void Selected(SprayObject _target)
    {
        target = _target;
        //transform.SetParent(target.transform);
        //((RectTransform)(transform)).localPosition = Vector3.zero;

        var pos = transform as RectTransform;
        var targetPos = target.transform as RectTransform;

        pos.SetSiblingIndex(targetPos.GetSiblingIndex());

        pos.localPosition = targetPos.localPosition;
        pos.localRotation = Quaternion.Euler(targetPos.eulerAngles);
        target.transform.SetParent(transform);
        targetPos.localPosition = Vector3.zero;
        targetPos.localRotation = Quaternion.identity;
        pos.sizeDelta = new Vector2(targetPos.sizeDelta.x + 2f * gap, targetPos.sizeDelta.y + 2f * gap);
        target.transform.SetAsFirstSibling();

        UiArrage();
        Active(true);
    }

    public void Deselected()
    {
        if (target)
        {
            eventRun = Stop;
            target.transform.SetParent(Touch.mainCanvas.transform);
            target.transform.SetSiblingIndex(transform.GetSiblingIndex());
            (target.transform as RectTransform).localPosition = (transform as RectTransform).localPosition;
            (target.transform as RectTransform).localRotation = (transform as RectTransform).localRotation;
            target = null;
            Active(false);
        }
    }

    public bool EqualTarget(SprayObject _target)
    { 
        return object.ReferenceEquals(target, _target);
    }

    public void EventStop()
    {
        eventRun = Stop;
    }

    void Stop() {}

    void PositionEvent()
    {
        //마우스 이벤트 ->터치 이벤트로 교체해야함
        #region 
        var handlePos = transform as RectTransform;
        handlePos.localPosition += UiMousePosition() - Touch.mainCanvas.transform.InverseTransformPoint(positionTab.position);
        #endregion
    }

    Vector3 UiMousePosition()
    {
        var mousePos = Input.mousePosition;
        mousePos.x -= 0.5f * Screen.width;
        mousePos.y -= 0.5f * Screen.height;
        return mousePos;
    }

    void RotationEvent()
    {
        var handleRT = transform as RectTransform;
        var rotTabDir = Touch.mainCanvas.transform.InverseTransformPoint(rotationTab.position)-handleRT.localPosition;
        var targetDir = UiMousePosition() - handleRT.localPosition;
        handleRT.localRotation = Quaternion.Euler(0f, 0f, handleRT.localEulerAngles.z+(Mathf.Atan2(targetDir.y,targetDir.x)-Mathf.Atan2(rotTabDir.y, rotTabDir.x))*Mathf.Rad2Deg);
    }

    void ScaleEvent()
    {
        var forceScale = (scaleTab.localPosition-(transform as RectTransform).InverseTransformPoint(
            Touch.mainCanvas.transform.TransformPoint(UiMousePosition()))) * 2f;
        target.SetForceScale(forceScale);
        var handleScale = (transform as RectTransform).sizeDelta;
        handleScale.x += forceScale.x;
        handleScale.y += forceScale.y;
        (transform as RectTransform).sizeDelta = handleScale;
        UiArrage();
    }

    void CloseEvent()
    {
        target.Destroy();
        target = null;
        eventRun = Stop;
        Active(false);
    }

    void FlipX_Event()
    {
        var rt = target.transform as RectTransform;
        rt.localScale = new Vector3(-rt.localScale.x, 1f, 1f);
        EventStop();
    }
    void RiseEvent()
    {
        transform.SetSiblingIndex(Touch.mainCanvas.transform.childCount - 2);
        EventStop();
    }

    public class EventTabSup
    {
        public static void PositionEventEnter(TSOHandle tsoHandle)
        {
            tsoHandle.eventRun = tsoHandle.PositionEvent;
        }
        public static void RotationEventEnter(TSOHandle tsoHandle)
        {
            tsoHandle.eventRun = tsoHandle.RotationEvent;
        }
        public static void ScaleEventEnter(TSOHandle tsoHandle)
        {
            tsoHandle.eventRun = tsoHandle.ScaleEvent;
        }
        public static void CloseEventEnter(TSOHandle tsoHandle)
        {
            tsoHandle.eventRun = tsoHandle.CloseEvent;
        }
        public static void FlipX_EventEnter(TSOHandle tsoHandle)
        {
            tsoHandle.eventRun = tsoHandle.FlipX_Event;
        }
        public static void RiseEventEnter(TSOHandle tsoHandle)
        {
            tsoHandle.eventRun = tsoHandle.RiseEvent;
        }
    }

    void Active(bool active)
    {
        gameObject.SetActive(active);
    }
}
