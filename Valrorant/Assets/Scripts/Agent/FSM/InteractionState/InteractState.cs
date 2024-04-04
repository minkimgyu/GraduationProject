using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class InteractState : State
{
    InteractionController _storedInteractionController;

    WeaponController _weaponController;

    public InteractState(InteractionController interactionController)
    {
        _storedInteractionController = interactionController;

        _weaponController = interactionController.GetComponentInParent<WeaponController>();
    }

    public override void OnStateEnter() 
    {
        AcquireWeapon();

        _storedInteractionController.InteractableTarget.OnSightExit();
        _storedInteractionController.InteractableTarget = null;

        _storedInteractionController.InteractionFSM.RevertToPreviousState(); // 이전 스테이트로 돌아감
    }

    void AcquireWeapon()
    {
        BaseWeapon weapon = _storedInteractionController.InteractableTarget.ReturnComponent<BaseWeapon>();
        if (weapon == null) return;

        _weaponController.StoredWeaponWhenInteracting = weapon;
    }
}
