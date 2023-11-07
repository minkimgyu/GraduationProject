using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StingerContainerClass : AbstractContainer<Stinger>, IWeaponContainer
{
    protected override void SetUp() { _storedRoutine.SetUp(this); }

    public BaseWeapon ReturnWeapon() { return _storedRoutine; }
}
