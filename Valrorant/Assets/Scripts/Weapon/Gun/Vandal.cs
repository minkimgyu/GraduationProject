using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

[System.Serializable]
public class Vandal : Gun
{
    [SerializeField]
    float _mainActionDelay;

    [SerializeField]
    float _subActionkDelay;

    [SerializeField]
    GameObject _scope;

    [SerializeField]
    float _zoomDuration;

    [SerializeField]
    float _scopeOnDelay;

    [SerializeField]
    RecoilMap _recoilMap;

    [SerializeField]
    float _normalFieldOfView;

    [SerializeField]
    float _zoomFieldOfView;

    [SerializeField]
    Vector3 _zoomCameraPosition;

    Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> _damageDictionary = new Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]>()
    {
        { DistanceAreaData.HitArea.Head, new DistanceAreaData[]{ new DistanceAreaData(0, 50, 160) } },
        { DistanceAreaData.HitArea.Body, new DistanceAreaData[]{ new DistanceAreaData(0, 50, 40) } },
        { DistanceAreaData.HitArea.Leg, new DistanceAreaData[]{ new DistanceAreaData(0, 50, 34) } },
    };


    public override void Initialize(GameObject player, Transform cam, Animator ownerAnimator)
    {
        base.Initialize(player, cam, ownerAnimator);

        // 여기에 Action 연결해서 총알이 소모되는 부분을 구현해보자
        _mainActionStrategy = new AutoAttackAction(_mainActionDelay);
        _subActionStrategy = new ManualAction(_subActionkDelay);

        _mainResultStrategy = new SingleProjectileAttack(_camTransform, _range, _targetLayer, ownerAnimator, _animator, _muzzleFlash, false,
            _emptyCartridgeSpawner, true, _weaponName.ToString(), _muzzle, _penetratePower, _trajectoryLineEffect, _damageDictionary);

        // 무기를 버릴 경우, 제거해야함
        _subResultStrategy = new ZoomStrategy(_scope, _zoomCameraPosition, _zoomDuration, _scopeOnDelay, _normalFieldOfView, _zoomFieldOfView, OnZoomEventCall);

        _mainRecoilStrategy = new AutoRecoilGenerator(_mainActionDelay, _recoilMap);
        _subRecoilStrategy = new NoRecoilGenerator();

        _reloadStrategy = new MagazineReload(_reloadFinishTime, _reloadExitTime, _weaponName.ToString(), _animator, _ownerAnimator, OnReloadRequested);

        _scope.SetActive(false);
        LinkEvent(player);
    }
}
