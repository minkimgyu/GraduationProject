using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;

namespace AI.SwatFSM
{
    public class IdleState : State
    {
        Action<Helper.BattleState> SetState;
        Action<BaseWeapon.Type> EquipWeapon;
        Func<bool> IsTargetInSight;
        Func<BaseWeapon.Type, BaseWeapon> ReturnWeapon;

        public IdleState(Action<Helper.BattleState> SetState, SwatBattleBlackboard blackboard)
        {
            this.SetState = SetState;
            EquipWeapon = blackboard.EquipWeapon;
            IsTargetInSight = blackboard.IsTargetInSight;
            ReturnWeapon = blackboard.ReturnWeapon;
        }

        public override void OnStateEnter()
        {
            BaseWeapon mainWeapon = ReturnWeapon(BaseWeapon.Type.Main);
            if (mainWeapon.IsAmmoEmpty() == false)
            {
                EquipWeapon(BaseWeapon.Type.Main);
                return;
            }

            BaseWeapon subWeapon = ReturnWeapon(BaseWeapon.Type.Sub);
            if (subWeapon.IsAmmoEmpty() == false)
            {
                EquipWeapon(BaseWeapon.Type.Sub);
                return;
            }
        }

        public override void CheckStateChange()
        {
            bool isInSight = IsTargetInSight();
            if (isInSight == false) return;

            SetState(Helper.BattleState.Attack);
        }
    }
}