using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

public class Knife : BaseWeapon
{
    [SerializeField]
    float _mainAttackDelay;

    [SerializeField]
    float _subAttackDelay;

    [SerializeField] float _delayWhenOtherSideAttack;

    [SerializeField]
    float attackLinkDuration = 2.8f;

    [SerializeField]
    int mainAnimationCount;

    [SerializeField]
    float mainAttackEffectDelayTime;

    [SerializeField]
    float subAttackEffectDelayTime;

    [SerializeField]
    DirectionData _mainAttackDamageData;

    [SerializeField]
    DirectionData _subAttackDamageData;

    public override void OnEquip()
    {
        base.OnEquip();
        OnRoundChangeRequested?.Invoke(false, 0, 0);
    }

    public override void Initialize(KnifeData data)
    {
        _eventStrategies[EventType.Main] = new AutoEvent(EventType.Main, _mainAttackDelay, OnEventStart, OnEventUpdate, OnEventEnd, OnAction);
        _eventStrategies[EventType.Sub] = new AutoEvent(EventType.Sub, _subAttackDelay, OnEventStart, OnEventUpdate, OnEventEnd, OnAction);

        _actionStrategies[EventType.Main] = new LeftKnifeAttack(_weaponName, _range, _targetLayer,
           mainAnimationCount, mainAttackEffectDelayTime, attackLinkDuration, _mainAttackDamageData, OnPlayWeaponAnimation);

        _actionStrategies[EventType.Sub] = new RightKnifeAttack(_weaponName, _range, _targetLayer,
           subAttackEffectDelayTime, _mainAttackDamageData, OnPlayWeaponAnimation);

        _recoilStrategies[EventType.Main] = new NoRecoilGenerator();
        _recoilStrategies[EventType.Sub] = new NoRecoilGenerator();
        _reloadStrategy = new NoReload();
    }
}
