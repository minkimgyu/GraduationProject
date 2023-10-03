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
        _mainAction = new AutoAttackAction(_mainAttackDelay); // 여기에 Action 연결해서 총알이 소모되는 부분을 구현해보자
        _subAction = new SingleAttactAction( _subAttackDelay);

        singleAttack = new SingleProjectileAttack(_camTransform, _range, _hitEffectName,
            _targetLayer, _weaponOwner, _muzzle, _penetratePower, _nonPenetrateHitEffect, _trajectoryLineEffect);

        noAttack = new NoAttack();

        // Action 등록해주기

        // --> 추가로 탄환 사용 부분 등록해주기
        _mainAction.OnActionStart = ChainMainAction;
        _subAction.OnActionStart = ChainSubAction;
    }

    protected override void ChainMainAction()
    {
        if (_bulletCountInMagazine <= 0) return;

        base.ChainMainAction();

        // 여기서 총알 감소 부분 추가해주기
        _bulletCountInMagazine -= mainAttackBulletCount; // 1발 발사

        singleAttack.Attack();
        OnAttack();
    }

    protected override void ChainSubAction()
    {
        base.ChainSubAction();
        noAttack.Attack();
    }
}
