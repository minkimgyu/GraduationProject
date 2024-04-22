using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;
using UnityEngine.UI;

public class Operator : VariationGun
{
    [SerializeField]
    float _mainActionDelayWhenZoomIn;

    [SerializeField]
    float _mainActionDelayWhenZoomOut;

    [SerializeField] protected int _fireCnt;

    [SerializeField] float _penetratePower;

    [SerializeField] protected float _recoveryDuration;


    [SerializeField]
    float _subActionDelay;

    [SerializeField]
    float _mainActionbulletSpreadPowerRatio;

    GameObject _scope;

    [SerializeField]
    float _meshDisableDelay;

    [SerializeField]
    float _zoomDuration;

    [SerializeField]
    float _scopeOnDelay;

    [SerializeField]
    float _normalFieldOfView;

    [SerializeField]
    float _zoomFieldOfView;

    [SerializeField]
    float _doubleZoomFieldOfView;

    [SerializeField]
    Vector3 _zoomCameraPosition;

    [SerializeField]
    GameObject _gunMesh;

    [SerializeField]
    WeightApplier _mainWeightApplier;

    Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> _damageDictionary = new Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]>()
    {
        { DistanceAreaData.HitArea.Head, new DistanceAreaData[]{ new DistanceAreaData(0, 50, 255) } },
        { DistanceAreaData.HitArea.Body, new DistanceAreaData[]{ new DistanceAreaData(0, 50, 150) } },
        { DistanceAreaData.HitArea.Leg, new DistanceAreaData[]{ new DistanceAreaData(0, 50, 120) } },
    };

    public override void Initialize(OperatorData data, RecoilRangeData mainRangeData, RecoilRangeData subRangeData)
    {
        _ammoCountsInMagazine = data.maxAmmoCountInMagazine;
        _ammoCountsInPossession = data.maxAmmoCountsInPossession;

        _reloadFinishDuration = data.reloadFinishDuration;
        _reloadExitDuration = data.reloadExitDuration;

        _eventStorage.Add(new(EventType.Main, Conditon.ZoomIn),
            new ManualEvent(EventType.Main, _mainActionDelayWhenZoomIn, OnEventStart, OnEventUpdate, OnEventEnd, OnAction));

        _eventStorage.Add(new(EventType.Main, Conditon.ZoomOut),
            new ManualEvent(EventType.Main, _mainActionDelayWhenZoomOut, OnEventStart, OnEventUpdate, OnEventEnd, OnAction));

        _eventStorage.Add(new(EventType.Sub, Conditon.Both),
            new ManualEvent(EventType.Sub, _subActionDelay, OnEventStart, OnEventUpdate, OnEventEnd, OnAction));


        _actionStorage.Add(new(EventType.Main, Conditon.ZoomIn),
            new SingleProjectileAttack(_weaponName, _range, _targetLayer, _fireCnt,
            _penetratePower, _mainActionbulletSpreadPowerRatio, _damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest));

        _actionStorage.Add(new(EventType.Main, Conditon.ZoomOut),
            new SingleProjectileAttackWithWeight(_weaponName, _range, _targetLayer, _fireCnt,
            _penetratePower, _mainActionbulletSpreadPowerRatio, _mainWeightApplier, _damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest));

        _actionStorage.Add(new(EventType.Sub, Conditon.Both),
            new DoubleZoomStrategy(_zoomCameraPosition, _zoomDuration, _normalFieldOfView, _zoomFieldOfView, _doubleZoomFieldOfView, OnZoomRequested));

        _recoilStorage.Add(new(EventType.Main, Conditon.ZoomIn),
            new ManualRecoilGenerator(_mainActionDelayWhenZoomIn, _recoveryDuration, mainRangeData));

        _recoilStorage.Add(new(EventType.Main, Conditon.ZoomOut),
            new ManualRecoilGenerator(_mainActionDelayWhenZoomOut, _recoveryDuration, subRangeData));

        _recoilStorage.Add(new(EventType.Sub, Conditon.Both), new NoRecoilGenerator());

        _reloadStrategy = new MagazineReload(_weaponName, _reloadFinishDuration, _reloadExitDuration, _maxAmmoCountInMagazine, OnPlayWeaponAnimation, OnReloadRequested);
    }
}
