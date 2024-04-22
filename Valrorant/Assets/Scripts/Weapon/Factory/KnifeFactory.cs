using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

[System.Serializable]
public class KnifeData
{
    public float mainAttackDelay;

    public float subAttackDelay;

    public float delayWhenOtherSideAttack;

    public float attackLinkDuration;

    public int mainAnimationCount;

    public float mainAttackEffectDelayTime;

    public float subAttackEffectDelayTime;

    public DirectionData mainAttackDamageData;

    public DirectionData subAttackDamageData;
}

public class KnifeFactory : WeaponFactory<KnifeData>
{
    public override BaseWeapon Create()
    {
        BaseWeapon weapon = Object.Instantiate(_prefab).GetComponent<BaseWeapon>();

        weapon.SetDefaultValue(); // 기본 값들을 초기화 해준다.
        weapon.Initialize(_data); // 무기를 초기화시킨다.
        weapon.MatchStrategy(); // Strategy를 매칭해준다.
        return weapon;
    }
}
