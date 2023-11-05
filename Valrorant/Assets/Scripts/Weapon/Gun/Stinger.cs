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
    float _subActionDelay;

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

    [SerializeField]
    WeightApplier _weightApplier;

    [SerializeField]
    RecoilRange _recoilRange;

    public override void Initialize(GameObject player, Transform cam, Animator ownerAnimator)
    {
        base.Initialize(player, cam, ownerAnimator);

        // 여기에 Action 연결해서 총알이 소모되는 부분을 구현해보자
        _storedMainActionStrategyWhenZoomOut = new AutoAttackAction(_autoAttackActionDelay);
        _storedMainActionStrategyWhenZoomIn = new BurstAttackAction(_burstAttackActionDelay, _burstAttackRecoverDuration, 4);
        // 두 개를 스위칭해서 사용하자

        _subActionStrategy = new ManualAction(_subActionDelay); // 줌 액션


        _storedMainResultStrategyWhenZoomOut = new SingleProjectileAttack(_camTransform, _range, _targetLayer, ownerAnimator, _animator, _muzzleFlash, false,
            _emptyCartridgeSpawner, true, _weaponName.ToString(), _muzzle, _penetratePower, _trajectoryLineEffect, _damageDictionary);

        _storedMainResultStrategyWhenZoomIn = new SingleProjectileAttackWithWeight(_camTransform, _range, _targetLayer, ownerAnimator, _animator, _muzzleFlash, false,
            _emptyCartridgeSpawner, true, _weaponName.ToString(), _muzzle, _penetratePower, _trajectoryLineEffect, _damageDictionary, _weightApplier);


        // 무기를 버릴 경우, 제거해야함
        _subResultStrategy = new ZoomStrategy(_scope, _zoomCameraPosition, _zoomDuration, _scopeOnDelay, _normalFieldOfView, _zoomFieldOfView, OnZoomEventCall); // OnZoomRequested 함수 넣어도 괜찮을 듯

        _storedMainRecoilStrategyWhenZoomOut = new AutoRecoilGenerator(_autoAttackActionDelay, _recoilMap);
        _storedMainRecoilStrategyWhenZoomIn = new BurstRecoilGenerator(_burstAttackActionDelay, _recoilRange);

        _subRecoilStrategy = new NoRecoilGenerator();

        _reloadStrategy = new MagazineReload(_reloadFinishTime, _reloadExitTime, _weaponName.ToString(), _animator, _ownerAnimator, OnReloadRequested);
        LinkEvent(player); // 이거 없애고 매개변수로 넘겨서 링크시키는 방식으로 진행
    }
}
