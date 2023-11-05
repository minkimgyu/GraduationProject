using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VandalContainerClass : AbstractContainer<Vandal>
{
    protected override void SetUp() { _storedRoutine.SetUp(this); }

    public override BaseWeapon ReturnWeapon() { return _storedRoutine; }
}
