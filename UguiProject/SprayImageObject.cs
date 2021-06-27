using UnityEngine;
using UnityEngine.UI;

public class SprayImageObject : SprayObject
{
    RectTransform rectTransform;

    void Start()
    {
        rectTransform = transform as RectTransform;
        var sizeDelta = rectTransform.sizeDelta;
        sizeDelta.x *= DB_Manager.dbManager.ScreenScaleLerp.x;
        sizeDelta.y *= DB_Manager.dbManager.ScreenScaleLerp.y;
        rectTransform.sizeDelta = sizeDelta;
        gameObject.SetActive(false);
    }

    public override void Create()
    {
        var s = DB_Manager.dbManager.PopSIO();
        if (s)
        {
            s.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
            s.rectTransform.sizeDelta = rectTransform.sizeDelta;
            s.rectTransform.SetParent(Touch.mainCanvas.transform);
            s.rectTransform.localPosition = Vector3.zero;
            s.rectTransform.localRotation = Quaternion.identity;
            s.rectTransform.localScale = new Vector3(1f, 1f, 1f);

            s.rectTransform.SetSiblingIndex(s.rectTransform.GetSiblingIndex() - 1);
            s.gameObject.SetActive(true);
        }
    }

    public override void Destroy()
    {
        gameObject.transform.SetParent(null);
        gameObject.SetActive(false);
        DB_Manager.dbManager.PushSIO(this);
    }

    public override void OnClick()
    {
        Touch.LinkSprayObject(this);
    }

    public override void SetForcePos(Vector3 pos)
    {
        rectTransform.localPosition += pos;
    }

    public override void SetForceRot(float rot)
    {
    }

    public override void SetForceScale(Vector3 scale)
    {
        rectTransform.sizeDelta += new Vector2(scale.x, scale.y);
    }
}
