using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class RecoilData : ScriptableObject
{
    [SerializeField]
    float _recoilRecoverDuration;

    public float RecoilRecoverDuration { get { return _recoilRecoverDuration; } }
}