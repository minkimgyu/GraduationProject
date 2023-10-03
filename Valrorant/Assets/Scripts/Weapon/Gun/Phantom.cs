using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phantom : Gun
{
    [SerializeField]
    int mainAttackBulletCount = 1;

    [SerializeField]
    float _mainAttackDelay;

    [SerializeField]
    float _subAttackDelay;

    AttackStrategy singleAttack;
    AttackStrategy noAttack;

    void Start()
    {
        _mainAction = new AutoAttackAction(_mainAttackDelay); // ���⿡ Action �����ؼ� �Ѿ��� �Ҹ�Ǵ� �κ��� �����غ���
        _subAction = new SingleAttactAction( _subAttackDelay);

        singleAttack = new SingleProjectileAttack(_camTransform, _range, _hitEffectName,
            _targetLayer, _weaponOwner, _muzzle, _penetratePower, _nonPenetrateHitEffect, _trajectoryLineEffect);

        noAttack = new NoAttack();

        // Action ������ֱ�

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
        base.ChainSubAction();
        noAttack.Attack();
    }
}
