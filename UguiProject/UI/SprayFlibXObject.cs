using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayFlibXObject : SprayTabObject
{
    public override void OnClick()
    {
        Touch.FlipX_EventActive();
    }
}
