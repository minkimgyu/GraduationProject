using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Agent.Controller;
using System;

namespace Agent.States
{
    public class InteractState : State
    {
        Action RevertToPreviousState;
        Action<BaseWeapon> SendWeaponToController;

        Action<IInteractable> ResetTarget;
        Func<IInteractable> ReturnTarget;

        public InteractState(Action RevertToPreviousState, Action<BaseWeapon> SendWeaponToController, Action<IInteractable> ResetTarget, Func<IInteractable> ReturnTarget)
        {
            this.RevertToPreviousState = RevertToPreviousState;
            this.SendWeaponToController = SendWeaponToController;

            this.ResetTarget = ResetTarget;
            this.ReturnTarget = ReturnTarget;
        }

        public override void OnStateEnter()
        {
            AcquireWeapon();

            IInteractable target = ReturnTarget();

            target.OnSightExit();
            ResetTarget(null);

            RevertToPreviousState(); // 이전 스테이트로 돌아감
        }

        void AcquireWeapon()
        {
            IInteractable target = ReturnTarget();

            BaseWeapon weapon = target.ReturnComponent<BaseWeapon>();
            if (weapon == null) return;

            SendWeaponToController?.Invoke(weapon);
        }
    }
}