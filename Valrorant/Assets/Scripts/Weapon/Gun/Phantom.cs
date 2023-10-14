using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

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
    GameObject _scope;

    [SerializeField]
    float _zoomDuration;

    [SerializeField]
    float _scopeOnDelay;

    Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> _damageDictionary = new Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]>()
    {
        { DistanceAreaData.HitArea.Head, new DistanceAreaData[]{ new DistanceAreaData(0, 15, 156), new DistanceAreaData(15, 30, 140), new DistanceAreaData(30, 50, 124) } },
        { DistanceAreaData.HitArea.Body, new DistanceAreaData[]{ new DistanceAreaData(0, 15, 39), new DistanceAreaData(15, 30, 35), new DistanceAreaData(30, 50, 31) } },
        { DistanceAreaData.HitArea.Leg, new DistanceAreaData[]{ new DistanceAreaData(0, 15, 33), new DistanceAreaData(15, 30, 29), new DistanceAreaData(30, 50, 26) } },
    };

    public override void Initialize(Transform cam, Animator ownerAnimator)
    {
        base.Initialize(cam, ownerAnimator);

        _mainAction = new AutoAttackAction(_mainAttackDelay); // 여기에 Action 연결해서 총알이 소모되는 부분을 구현해보자
        _subAction = new SingleAttactAction(_subAttackDelay);

        _mainResult = new SingleProjectileAttack(_camTransform, _range, _hitEffectName,
            _targetLayer, _muzzle, _penetratePower, _nonPenetrateHitEffect, _trajectoryLineEffect, _damageDictionary);

        _subResult = new Zoom(_aimEvent, _scope, _zoomDuration, _scopeOnDelay);

        _mainAction.OnActionStart = ChainMainAction;
        _subAction.OnActionStart = ChainSubAction;

        _scope.SetActive(false);
    }

    public override void OnUnEquip()
    {
        base.OnUnEquip();
        _aimEvent.RaiseEvent(_scope, false, _zoomDuration, _scopeOnDelay);
    }

    public override void OffScopeModeInstantly()
    {
        _aimEvent.RaiseEvent(_scope, false);
    }

    protected override void ChainMainAction()
    {
        if (_bulletCountInMagazine <= 0) return;

        base.ChainMainAction();

        // 여기서 총알 감소 부분 추가해주기
        _bulletCountInMagazine -= mainAttackBulletCount; // 1발 발사

        _mainResult.Do();
        OnAttack();
    }

    protected override void ChainSubAction()
    {
        _subResult.Do();
    }
}
