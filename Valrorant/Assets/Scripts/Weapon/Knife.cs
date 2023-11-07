using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

[System.Serializable]
public class Knife : NoVariationWeapon
{
    [SerializeField]
    float _mainAttackDelay;

    [SerializeField]
    float _subAttackDelay;

    [SerializeField]
    float attackLinkTime = 2.8f;

    [SerializeField]
    float[] mainAttackEffectDelayTime;

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

    public override void Initialize(GameObject player, GameObject armMesh, Transform cam, Animator ownerAnimator)
    {
        base.Initialize(player, armMesh, cam, ownerAnimator);

        _mainResultStrategy = new LeftKnifeAttack(_camTransform, _range, _targetLayer, ownerAnimator, _animator, true, WeaponName.ToString(), 
            mainAttackEffectDelayTime, _subAttackDelay, attackLinkTime, _mainAttackDamageData);

        _subResultStrategy = new RightKnifeAttack(_camTransform, _range, _targetLayer, ownerAnimator, _animator, false, WeaponName.ToString(),
            subAttackEffectDelayTime, _mainAttackDelay, _subAttackDamageData);

        _mainActionStrategy = new AutoAttackAction(_mainAttackDelay);
        _subActionStrategy = new AutoAttackAction(_subAttackDelay);

        _mainRecoilStrategy = new NoRecoilGenerator();
        _subRecoilStrategy = new NoRecoilGenerator();
        _reloadStrategy = new NoReload();

        LinkEvent(player);
    }
}
