using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

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

    Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> _attackDamageDictionary = new Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]>()
    {
        { DistanceAreaData.HitArea.Head, new DistanceAreaData[]{ new DistanceAreaData(0, 30, 78), new DistanceAreaData(30, 50, 66) } },
        { DistanceAreaData.HitArea.Body, new DistanceAreaData[]{ new DistanceAreaData(0, 30, 26), new DistanceAreaData(30, 50, 22) } },
        { DistanceAreaData.HitArea.Leg, new DistanceAreaData[]{ new DistanceAreaData(0, 30, 22), new DistanceAreaData(30, 50, 18) } },
    };

    public override void Initialize(Transform cam, Animator ownerAnimator)
    {
        base.Initialize(cam, ownerAnimator);

        _mainResult = new SingleProjectileAttack(_camTransform, _range, _hitEffectName,
            _targetLayer, _muzzle, _penetratePower, _nonPenetrateHitEffect, _trajectoryLineEffect, _attackDamageDictionary);

        _subResult = new ScatterProjectileGunAttack(_camTransform, _range, _hitEffectName,
            _targetLayer, _muzzle, _penetratePower, _nonPenetrateHitEffect, _trajectoryLineEffect, _projectileCounts, _spreadOffset, _attackDamageDictionary);


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

        _mainResult.Do();
        OnAttack();
    }

    protected override void ChainSubAction()
    {
        if (_bulletCountInMagazine <= 0) return;

        base.ChainSubAction();

        // ���⼭ �Ѿ� ���� �κ� �߰����ֱ�
        _bulletCountInMagazine -= _projectileCounts; // 3�� �߻�

        _subResult.Do();
        OnAttack();
    }
}
