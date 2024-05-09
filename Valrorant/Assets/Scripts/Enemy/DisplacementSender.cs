using System;
using UnityEngine;

public class DisplacementSender : MonoBehaviour
{
    protected float _velocityLengthDecreaseRatio = 0.1f;

    public virtual float RaiseDisplacementEvent() { return 0; }
}
