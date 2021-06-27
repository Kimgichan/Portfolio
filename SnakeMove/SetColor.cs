using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColor : MonoBehaviour
{
    [SerializeField] Material[] mat;
    [SerializeField] Color col;
    // Start is called before the first frame update
    void Start()
    {
        foreach(var m in mat)
        {
            m.SetColor("_Col", col);
        }
    }
}
