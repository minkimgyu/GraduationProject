using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunContainerClass : AbstractContainer<Shotgun>, IWeaponContainer
{
    protected override void SetUp() { _storedRoutine.SetUp(this); }

    public BaseWeapon ReturnWeapon() { return _storedRoutine; }
}
