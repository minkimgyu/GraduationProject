using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DisplacementSender : MonoBehaviour
{
    public Action<float> OnDisplacementRequested;

    protected float _velocityLengthDecreaseRatio = 0.1f;

    public virtual void RaiseDisplacementEvent() { }
}
