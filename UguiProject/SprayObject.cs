using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SprayObject : MonoBehaviour
{
    public abstract void Create();
    public abstract void SetForcePos(Vector3 pos);
    public abstract void SetForceRot(float rot);
    public abstract void SetForceScale(Vector3 scale);
    public abstract void OnClick();
    public abstract void Destroy();
}