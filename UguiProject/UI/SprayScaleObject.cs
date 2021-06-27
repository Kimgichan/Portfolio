using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayScaleObject : SprayTabObject
{
    public override void OnClick()
    {
        Touch.ScaleEventActive();
    }
}
