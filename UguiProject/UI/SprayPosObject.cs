using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayPosObject : SprayTabObject
{
    public override void OnClick()
    {
        Touch.PositionEventActive();
    }
}
