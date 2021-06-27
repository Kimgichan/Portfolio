using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleLerpScrollView : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var rt = transform as RectTransform;
        var lerp = DB_Manager.dbManager.ScreenScaleLerp;
        rt.transform.localPosition = new Vector3(rt.transform.localPosition.x * lerp.x,
            rt.transform.localPosition.y * lerp.y, 0f);
        rt.sizeDelta = new Vector2(rt.sizeDelta.x * lerp.x, rt.sizeDelta.y * lerp.y);
    }
}
