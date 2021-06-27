using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyScrollRect : MonoBehaviour
{
    [SerializeField] ScrollRect scrollRect;
    public void Init()
    {
        scrollRect.verticalNormalizedPosition = ((transform as RectTransform).rect.height - scrollRect.content.rect.height) * 0.5f;
    }
}
