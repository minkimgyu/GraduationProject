using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct SwatBattleBlackboard
{
    public SwatBattleBlackboard(float attackDuration, float attackDelay, Func<bool> IsTargetInSight, Func<ISightTarget> ReturnTargetInSight, Action<BaseWeapon.Type> EquipWeapon, 
        Action<BaseWeapon.EventType> EventStart, Action EventEnd)
    {
        AttackDuration = attackDuration;
        AttackDelay = attackDelay;

        this.IsTargetInSight = IsTargetInSight;
        this.ReturnTargetInSight = ReturnTargetInSight;

        this.EquipWeapon = EquipWeapon;
        this.EventStart = EventStart;
        this.EventEnd = EventEnd;
    }

    public float AttackDuration { get; }
    public float AttackDelay { get; }

    public Func<bool> IsTargetInSight { get; }
    public Func<ISightTarget> ReturnTargetInSight { get; }

    public Action<BaseWeapon.Type> EquipWeapon { get; }
    public Action<BaseWeapon.EventType> EventStart { get; }
    public Action EventEnd { get; }
}
