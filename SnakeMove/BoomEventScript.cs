using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomEventScript : MonoBehaviour
{
    [SerializeField] ArrowScript arrow;
    // Start is called before the first frame update
    public void ArrowHide()
    {
        arrow.ArrowHide();
    }

    public void MenuActive()
    {
        arrow.MenuActive();
    }
}
