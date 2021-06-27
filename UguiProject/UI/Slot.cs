using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] string list;
    [SerializeField] string sprayName;

    public void OnClick()
    {
        DB_Manager.dbManager.CreateSprayObject(list, sprayName);
    }
}
