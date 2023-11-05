using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomContainerClass : AbstractContainer<Phantom>
{
    protected override void SetUp() { _storedRoutine.SetUp(this); }

    public override BaseWeapon ReturnWeapon() { return _storedRoutine; }
}