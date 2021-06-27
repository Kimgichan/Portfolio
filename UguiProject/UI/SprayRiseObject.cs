using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayRiseObject : SprayTabObject
{
    public override void OnClick()
    {
        Touch.RiseEventActive();
    }
}
