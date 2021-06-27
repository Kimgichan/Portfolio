using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemListEvent : MonoBehaviour
{
    [SerializeField] GameObject cat;
    [SerializeField] GameObject catList;
    [SerializeField] GameObject apple;
    [SerializeField] GameObject appleList;
    [SerializeField] GameObject human;
    [SerializeField] GameObject humanList;
    [SerializeField] GameObject background;
    [SerializeField] GameObject backgroundList;
    Action offHandle;

    // Start is called before the first frame update
    void Start()
    {
        offHandle = delegate () { };
        CatClick();
    }

    private void SetColor(GameObject go, Color color)
    {
        var image = go.gameObject.GetComponent<Image>();
        image.color = color;
    }
    public void CatClick()
    {
        offHandle();
        SetColor(catList, new Color(1f, 0.5f, 0.5f, 1f));
        cat.SetActive(true);
        offHandle = CatOffEvent;
    }
    void CatOffEvent()
    {
        SetColor(catList, new Color(1f, 1f, 1f, 1f));
        cat.SetActive(false);
    }
    public void AppleClick()
    {
        offHandle();
        SetColor(appleList, new Color(1f, 0.5f, 0.5f, 1f));
        apple.SetActive(true);
        offHandle = AppleOffEvent;
    }
    void AppleOffEvent()
    {
        SetColor(appleList, new Color(1f, 1f, 1f, 1f));
        apple.SetActive(false);
    }
    public void HumanClick()
    {
        offHandle();
        SetColor(humanList, new Color(1f, 0.5f, 0.5f, 1f));
        human.SetActive(true);
        offHandle = HumanOffEvent;
    }
    void HumanOffEvent()
    {
        SetColor(humanList, new Color(1f, 1f, 1f, 1f));
        human.SetActive(false);
    }
    public void BackgroundClick()
    {
        offHandle();
        SetColor(backgroundList, new Color(1f, 0.5f, 0.5f, 1f));
        background.SetActive(true);
        offHandle = BackgroundOffEvent;
    }
    void BackgroundOffEvent()
    {
        SetColor(backgroundList, new Color(1f, 1f, 1f, 1f));
        background.SetActive(false);
    }
}
