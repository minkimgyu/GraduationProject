using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;
using UnityEngine.UI;

public class Operator : VariationGun
{
    //[SerializeField]
    //float _mainActionDelayWhenZoomIn;

    //[SerializeField]
    //float _mainActionDelayWhenZoomOut;

    //[SerializeField] protected int _fireCnt;

    //[SerializeField] float _penetratePower;

    //[SerializeField] protected float _recoveryDuration;


    //[SerializeField]
    //float _subActionDelay;

    //[SerializeField]
    //float _mainActionbulletSpreadPowerRatio;

    //GameObject _scope;

    //[SerializeField]
    //float _meshDisableDelay;

    //[SerializeField]
    //float _zoomDuration;

    //[SerializeField]
    //float _scopeOnDelay;

    //[SerializeField]
    //float _normalFieldOfView;

    //[SerializeField]
    //float _zoomFieldOfView;

    //[SerializeField]
    //float _doubleZoomFieldOfView;

    //[SerializeField]
    //Vector3 _zoomCameraPosition;

    //[SerializeField]
    //GameObject _gunMesh;

    //[SerializeField]
    //WeightApplier _mainWeightApplier;

    //Dictionary<HitArea, DistanceAreaData[]> _damageDictionary = new Dictionary<HitArea, DistanceAreaData[]>()
    //{
    //    { HitArea.Head, new DistanceAreaData[]{ new DistanceAreaData(0, 50, 255) } },
    //    { HitArea.Body, new DistanceAreaData[]{ new DistanceAreaData(0, 50, 150) } },
    //    { HitArea.Leg, new DistanceAreaData[]{ new DistanceAreaData(0, 50, 120) } },
    //};

    public override void Initialize(OperatorData data, RecoilRangeData mainRangeData, RecoilRangeData subRangeData)
    {
        _ammoCountsInMagazine = data.maxAmmoCountInMagazine;
        _ammoCountsInPossession = data.maxAmmoCountsInPossession;

        _eventStorage.Add(new(EventType.Main, Conditon.ZoomIn),
            new ManualEvent(EventType.Main, data.mainActionDelayWhenZoomIn, OnEventStart, OnEventUpdate, OnEventEnd, OnAction));

        _eventStorage.Add(new(EventType.Main, Conditon.ZoomOut),
            new ManualEvent(EventType.Main, data.mainActionDelayWhenZoomOut, OnEventStart, OnEventUpdate, OnEventEnd, OnAction));

        _eventStorage.Add(new(EventType.Sub, Conditon.Both),
            new ManualEvent(EventType.Sub, data.subActionDelay, OnEventStart, OnEventUpdate, OnEventEnd, OnAction));


        _actionStorage.Add(new(EventType.Main, Conditon.ZoomIn),
            new SingleProjectileAttack(data.weaponName, data.range, _targetLayer, data.fireCnt,
            data.penetratePower, data.mainActionbulletSpreadPowerRatio, data.damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest));

        _actionStorage.Add(new(EventType.Main, Conditon.ZoomOut),
            new SingleProjectileAttackWithWeight(data.weaponName, data.range, _targetLayer, data.fireCnt,
            data.penetratePower, data.mainActionbulletSpreadPowerRatio, data.mainWeightApplier, data.damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest));

        _actionStorage.Add(new(EventType.Sub, Conditon.Both),
            new DoubleZoomStrategy(data.zoomCameraPosition.V3, data.zoomDuration, data.normalFieldOfView, data.zoomFieldOfView, data.doubleZoomFieldOfView, OnZoomRequested));

        _recoilStorage.Add(new(EventType.Main, Conditon.ZoomIn),
            new ManualRecoilGenerator(data.mainActionDelayWhenZoomIn, data.recoveryDuration, mainRangeData));

        _recoilStorage.Add(new(EventType.Main, Conditon.ZoomOut),
            new ManualRecoilGenerator(data.mainActionDelayWhenZoomOut, data.recoveryDuration, subRangeData));

        _recoilStorage.Add(new(EventType.Sub, Conditon.Both), new NoRecoilGenerator());

        _reloadStrategy = new MagazineReload(data.weaponName, data.reloadFinishDuration, data.reloadExitDuration, data.maxAmmoCountInMagazine, OnPlayWeaponAnimation, OnReloadRequested);
    }
}
