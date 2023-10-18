using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassicContainerClass : AbstractRoutineContainerClass
{
    [SerializeField]
    Classic _classic;

    protected override void Awake()
    {
        _baseAbstractRoutineClass = _classic;
        base.Awake();
    }
}
