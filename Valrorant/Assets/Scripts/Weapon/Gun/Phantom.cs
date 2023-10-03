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
        _mainAction = new AutoAttackAction(_mainAttackDelay); // 여기에 Action 연결해서 총알이 소모되는 부분을 구현해보자
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

        // 여기서 총알 감소 부분 추가해주기
        _bulletCountInMagazine -= mainAttackBulletCount; // 1발 발사

        _mainResult.Attack();
        OnAttack();
    }

    protected override void ChainSubAction()
    {
        _subResult.Attack();
    }
}
