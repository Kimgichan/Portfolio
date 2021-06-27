using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayCloseObject : SprayTabObject
{
    public override void OnClick()
    {
        Touch.CloseEventActive();
    }
}
