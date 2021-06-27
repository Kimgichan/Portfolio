using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTouchScript : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), -transform.forward);
            if (hit)
            {
                RouletteScript.Play();
            }
        }
    }
}
