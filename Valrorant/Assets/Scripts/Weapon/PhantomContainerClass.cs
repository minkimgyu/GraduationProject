using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomContainerClass : AbstractRoutineContainerClass
{
    [SerializeField]
    Phantom _phantom;

    protected override void Awake()
    {
        _baseAbstractRoutineClass = _phantom;
        base.Awake();
    }
}
