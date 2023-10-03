using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Classic : Gun
{
    [SerializeField]
    int mainAttackBulletCount = 1;

    [SerializeField]
    int _projectileCounts;

    [SerializeField]
    float _spreadOffset;

    [SerializeField]
    float _mainAttackDelay;

    [SerializeField]
    float _subAttackDelay;

    AttackStrategy singleAttack;
    AttackStrategy scatterAttack;

    void Start()
    {
        singleAttack = new SingleProjectileAttack(_camTransform, _range, _hitEffectName,
            _targetLayer, _weaponOwner, _muzzle, _penetratePower, _nonPenetrateHitEffect, _trajectoryLineEffect);

        scatterAttack = new ScatterProjectileGunAttack(_camTransform, _range, _hitEffectName,
            _targetLayer, _weaponOwner, _muzzle, _penetratePower, _nonPenetrateHitEffect, _trajectoryLineEffect, _projectileCounts, _spreadOffset);


        _mainAction = new SingleAttactAction(_mainAttackDelay);
        _subAction = new SingleAttactAction(_subAttackDelay);

        // --> �߰��� źȯ ��� �κ� ������ֱ�
        _mainAction.OnActionStart = ChainMainAction;
        _subAction.OnActionStart = ChainSubAction;
    }

    protected override void ChainMainAction()
    {
        if (_bulletCountInMagazine <= 0) return;

        base.ChainMainAction();

        // ���⼭ �Ѿ� ���� �κ� �߰����ֱ�
        _bulletCountInMagazine -= mainAttackBulletCount; // 1�� �߻�

        singleAttack.Attack();
        OnAttack();
    }

    protected override void ChainSubAction()
    {
        if (_bulletCountInMagazine <= 0) return;

        base.ChainSubAction();

        // ���⼭ �Ѿ� ���� �κ� �߰����ֱ�
        _bulletCountInMagazine -= _projectileCounts; // 3�� �߻�

        scatterAttack.Attack();
        OnAttack();
    }
}
