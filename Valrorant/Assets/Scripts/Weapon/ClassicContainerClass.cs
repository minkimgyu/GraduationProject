using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassicContainerClass : AbstractContainer<Classic>, IWeaponContainer, IInteractContainer
{
    protected override void SetUp() { _storedRoutine.SetUp(this); }

    public BaseWeapon ReturnWeapon() { return _storedRoutine; }

    public IInteractable ReturnInteractableObject() { return _storedRoutine; }
}
