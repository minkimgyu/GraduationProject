using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeContainerClass : AbstractRoutineContainerClass
{
    [SerializeField]
    Knife _knife;

    protected override void Awake()
    {
        _baseAbstractRoutineClass = _knife;
        base.Awake();
    }
}
