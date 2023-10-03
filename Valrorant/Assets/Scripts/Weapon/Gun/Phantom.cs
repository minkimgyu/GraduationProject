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

    [SerializeField]
    AimEvent _aimEvent;

    [SerializeField]
    GameObject scope;

    protected override void Awake()
    {
        base.Awake();
        _mainAction = new AutoAttackAction(_mainAttackDelay); // ���⿡ Action �����ؼ� �Ѿ��� �Ҹ�Ǵ� �κ��� �����غ���
        _subAction = new SingleAttactAction(_subAttackDelay);

        _mainResult = new SingleProjectileAttack(_camTransform, _range, _hitEffectName,
            _targetLayer, _weaponOwner, _muzzle, _penetratePower, _nonPenetrateHitEffect, _trajectoryLineEffect);

        _subResult = new Zoom(_aimEvent, scope);

        _mainAction.OnActionStart = ChainMainAction;
        _subAction.OnActionStart = ChainSubAction;
    }

    private void Start()
    {
        scope.SetActive(false);
    }

    protected override void ChainMainAction()
    {
        if (_bulletCountInMagazine <= 0) return;

        base.ChainMainAction();

        // ���⼭ �Ѿ� ���� �κ� �߰����ֱ�
        _bulletCountInMagazine -= mainAttackBulletCount; // 1�� �߻�

        _mainResult.Attack();
        OnAttack();
    }

    protected override void ChainSubAction()
    {
        _subResult.Attack();
    }
}
