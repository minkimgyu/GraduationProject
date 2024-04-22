using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

public class Stinger : VariationGun
{
    [SerializeField]
    float _autoFireInterval;

    [SerializeField]
    float _burstFireInterval;

    [SerializeField] protected float _penetratePower;

    [SerializeField] protected float _recoveryDuration;

    [SerializeField]
    float _zoomDelay;

    [SerializeField] int _burstFireCntInOneAction;

    [SerializeField] int _mainFireCnt;

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

    public override void Initialize(StingerData data, RecoilMapData mainMapData, RecoilRangeData subRangeData)
    {
        _ammoCountsInMagazine = data.maxAmmoCountInMagazine;
        _ammoCountsInPossession = data.maxAmmoCountsInPossession;

        _reloadFinishDuration = data.reloadFinishDuration;
        _reloadExitDuration = data.reloadExitDuration;

        _eventStorage.Add(new(EventType.Main, Conditon.ZoomIn),
            new BurstEvent(EventType.Main, _burstFireInterval, _burstFireCntInOneAction, OnEventStart, OnEventUpdate, OnEventEnd, OnAction));

        _eventStorage.Add(new(EventType.Main, Conditon.ZoomOut),
            new AutoEvent(EventType.Main, _autoFireInterval, OnEventStart, OnEventUpdate, OnEventEnd, OnAction));

        _eventStorage.Add(new(EventType.Sub, Conditon.Both),
            new ManualEvent(EventType.Sub, _zoomDelay, OnEventStart, OnEventUpdate, OnEventEnd, OnAction));


        _actionStorage.Add(new(EventType.Main, Conditon.ZoomIn),
            new SingleProjectileAttackWithWeight(_weaponName, _range, _targetLayer, _mainFireCnt,
            _penetratePower, _bulletSpreadPowerDecreaseRatio, _weightApplier, _damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest));

        _actionStorage.Add(new(EventType.Main, Conditon.ZoomOut),
             new SingleProjectileAttack(_weaponName, _range, _targetLayer, _mainFireCnt,
             _penetratePower, _bulletSpreadPowerDecreaseRatio, _damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
             SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest));


        _actionStorage.Add(new(EventType.Sub, Conditon.Both),
            new ZoomStrategy(_zoomCameraPosition, _zoomDuration, _normalFieldOfView, _zoomFieldOfView, OnZoomRequested));



        _recoilStorage.Add(new(EventType.Main, Conditon.ZoomOut),
            new AutoRecoilGenerator(_autoFireInterval, _recoveryDuration, _recoilRatio, mainMapData));

        _recoilStorage.Add(new(EventType.Main, Conditon.ZoomIn),
            new BurstRecoilGenerator(_burstFireInterval, _recoveryDuration, subRangeData));

        _recoilStorage.Add(new(EventType.Sub, Conditon.Both), new NoRecoilGenerator());


        _reloadStrategy = new MagazineReload(_weaponName, _reloadFinishDuration, _reloadExitDuration, _maxAmmoCountInMagazine, OnPlayWeaponAnimation, OnReloadRequested);
    }
}
