using UnityEngine;
using UnityEngine.UI;

public class SprayTextObject : SprayObject
{
    InputField inputField;
    RectTransform rectTransform;
    Text text;

    public override void Create()
    {

    }

    public override void Destroy()
    {
    }

    public void Init(Color color)
    {
        inputField = GetComponent<InputField>();
        rectTransform = GetComponent<RectTransform>();
        text = transform.GetChild(1).gameObject.GetComponent<Text>();

        text.color = color;
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
    }
}
