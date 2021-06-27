using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hint : MonoBehaviour
{
    [SerializeField] GameObject ui;

    public void HintClick()
    {
        ui.SetActive(true);
    }

    public void HintClose()
    {
        ui.SetActive(false);
    }
}
