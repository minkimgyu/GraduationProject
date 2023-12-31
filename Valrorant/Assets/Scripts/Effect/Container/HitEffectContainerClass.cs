using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffectContainerClass : EffectContainer<HitEffect>, IEffectContainer
{
    protected override void SetUp() { _storedRoutine.SetUp(this); }

    public BaseEffect ReturnEffect() { return _storedRoutine; }
}
