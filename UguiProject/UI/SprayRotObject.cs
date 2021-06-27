using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayRotObject : SprayTabObject
{
    public override void OnClick()
    {
        Touch.RotationEventActive();
    }
}
