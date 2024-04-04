using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

[System.Serializable]
public class Stinger : AllVariationGun
{
    [SerializeField]
    float _autoAttackActionDelay;

    [SerializeField]
    float _burstAttackActionDelay;

    [SerializeField]
    float _burstAttackRecoverDuration; // 이게 좀 더 길어야함

    [SerializeField]
    float _bulletSpreadPowerDecreaseRatio;

    [SerializeField]
    float _recoilRatio;


    [SerializeField]
    float _subActionDelay;

    [SerializeField]
    GameObject _scope;

    [SerializeField]
    float _zoomDuration;

    [SerializeField]
    float _scopeOnDelay;

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

    [SerializeField]
    WeightApplier _weightApplier;

    public override void Initialize(GameObject player, GameObject armMesh, Transform cam, Animator ownerAnimator)
    {
        base.Initialize(player, armMesh, cam, ownerAnimator);

        // 여기에 Action 연결해서 총알이 소모되는 부분을 구현해보자
        _storedMainActionWhenZoomOut = new AutoAttackAction(_autoAttackActionDelay);
        _storedMainActionWhenZoomIn = new BurstAttackAction(_burstAttackActionDelay, _burstAttackRecoverDuration, 4);
        // 두 개를 스위칭해서 사용하자

        _subActionStrategy = new ManualAction(_subActionDelay); // 줌 액션


        _storedMainResultWhenZoomOut = new SingleProjectileAttack(_camTransform, _range, _targetLayer, ownerAnimator, _animator, _muzzleFlash, false,
            _emptyCartridgeSpawner, true, _weaponName.ToString(), _muzzle, _penetratePower, _trajectoryLineEffect, _bulletSpreadPowerDecreaseRatio, _damageDictionary);

        _storedMainResultWhenZoomIn = new SingleProjectileAttackWithWeight(_camTransform, _range, _targetLayer, ownerAnimator, _animator, _muzzleFlash, false,
            _emptyCartridgeSpawner, true, _weaponName.ToString(), _muzzle, _penetratePower, _trajectoryLineEffect, _bulletSpreadPowerDecreaseRatio, _damageDictionary, _weightApplier);


        // 무기를 버릴 경우, 제거해야함
        _subResultStrategy = new ZoomStrategy(_scope, _zoomCameraPosition, _zoomDuration, _scopeOnDelay, _normalFieldOfView, _zoomFieldOfView, OnZoomEventCall); // OnZoomRequested 함수 넣어도 괜찮을 듯

        RecoilStorage storage = GameObject.FindWithTag("RecoilStorage").GetComponent<RecoilStorage>();
        RecoilMapData mainRecoilData = storage.OnRecoilDataSendRequested<RecoilMapData>(_weaponName, EventCallPart.Left);
        RecoilRangeData subRecoilData = storage.OnRecoilDataSendRequested<RecoilRangeData>(_weaponName, EventCallPart.Right);

        _storedMainRecoilWhenZoomOut = new AutoRecoilGenerator(_autoAttackActionDelay, _recoilRatio, mainRecoilData);
        _storedMainRecoilWhenZoomIn = new BurstRecoilGenerator(_burstAttackActionDelay, subRecoilData);

        _subRecoilStrategy = new NoRecoilGenerator();

        _reloadStrategy = new MagazineReload(_reloadFinishTime, _reloadExitTime, _weaponName.ToString(), _maxAmmoCountInMagazine, _animator, _ownerAnimator, OnReloadRequested);
        LinkEvent(player); // 이거 없애고 매개변수로 넘겨서 링크시키는 방식으로 진행
    }
}
