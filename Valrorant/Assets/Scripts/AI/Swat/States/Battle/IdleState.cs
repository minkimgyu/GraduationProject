using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;

namespace AI.SwatFSM
{
    public class IdleState : State
    {
        Action<Swat.BattleState> SetState;
        Action<BaseWeapon.Type> EquipWeapon;
        Func<bool> IsTargetInSight;

        public IdleState(Action<Swat.BattleState> SetState, SwatBattleBlackboard blackboard)
        {
            this.SetState = SetState;
            EquipWeapon = blackboard.EquipWeapon;
            IsTargetInSight = blackboard.IsTargetInSight;
        }

        public override void OnStateEnter()
        {
            EquipWeapon(BaseWeapon.Type.Main);
        }

        public override void CheckStateChange()
        {
            bool isInSight = IsTargetInSight();
            if (isInSight == false) return;

            SetState(Swat.BattleState.Attack);
        }
    }
}