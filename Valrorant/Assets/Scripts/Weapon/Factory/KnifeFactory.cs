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

        weapon.SetDefaultValue(); // �⺻ ������ �ʱ�ȭ ���ش�.
        weapon.Initialize(_data); // ���⸦ �ʱ�ȭ��Ų��.
        weapon.MatchStrategy(); // Strategy�� ��Ī���ش�.
        return weapon;
    }
}
