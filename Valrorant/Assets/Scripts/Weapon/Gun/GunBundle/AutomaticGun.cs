using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;
using System;

abstract public class AutomaticGun : VariationGun
{
    [SerializeField] protected float _fireIntervalWhenZoomIn;
    [SerializeField] protected float _fireIntervalWhenZoomOut;

    [SerializeField] protected int _fireCnt;
    [SerializeField] protected float _penetratePower;

    [SerializeField] protected float _recoveryDuration;

    [SerializeField] protected float _recoilRatioWhenZoomIn = 1.0f;
    [SerializeField] protected float _recoilRatioWhenZoomOut = 0.5f;

    [SerializeField] protected float _zoomDelay;

    /// <summary>
    /// 움직임에 따라 변경되는 값
    /// </summary>
    [SerializeField] protected float _displacementSpreadMultiplyRatio = 1;

    [SerializeField] protected float _zoomDuration = 0.5f;

    [SerializeField] protected float _normalFieldOfView;

    [SerializeField] protected float _zoomFieldOfView;

    [SerializeField] protected Vector3 _cameraPositionWhenZoom;

    protected Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> _damageDictionary;

    public override void Initialize(AutomaticGunData data, RecoilMapData recoilData)
    {
        _ammoCountsInMagazine = data.maxAmmoCountInMagazine;
        _ammoCountsInPossession = data.maxAmmoCountsInPossession;

        _reloadFinishDuration = data.reloadFinishDuration;
        _reloadExitDuration = data.reloadExitDuration;

        _eventStorage.Add(new(EventType.Main, Conditon.ZoomIn), 
            new AutoEvent(EventType.Main, data.fireIntervalWhenZoomIn, OnEventStart, OnEventUpdate, OnEventEnd, OnAction));

        _eventStorage.Add(new(EventType.Main, Conditon.ZoomOut)
            , new AutoEvent(EventType.Main, data.fireIntervalWhenZoomOut, OnEventStart, OnEventUpdate, OnEventEnd, OnAction));

        _eventStorage.Add(new(EventType.Sub, Conditon.Both)
            , new ManualEvent(EventType.Sub, data.zoomDelay, OnEventStart, OnEventUpdate, OnEventEnd, OnAction));


        _actionStorage.Add(new(EventType.Main, Conditon.Both),
            new SingleProjectileAttack(data.weaponName, data.range, _targetLayer, data.fireCnt,
            data.penetratePower, data.displacementSpreadMultiplyRatio, data.damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest));

        _actionStorage.Add(new(EventType.Sub, Conditon.Both),
            new ZoomStrategy(data.cameraPositionWhenZoom, data.zoomDuration, data.normalFieldOfView, data.zoomFieldOfView, OnZoomRequested));


        _recoilStorage.Add(new(EventType.Main, Conditon.ZoomIn),
            new AutoRecoilGenerator(data.fireIntervalWhenZoomIn, data.recoveryDuration, data.recoilRatioWhenZoomIn, recoilData));

        _recoilStorage.Add(new(EventType.Main, Conditon.ZoomOut),
            new AutoRecoilGenerator(data.fireIntervalWhenZoomIn, data.recoveryDuration, data.recoilRatioWhenZoomOut, recoilData));

        _recoilStorage.Add(new(EventType.Sub, Conditon.Both), new NoRecoilGenerator());

        _reloadStrategy = new MagazineReload(data.weaponName, data.reloadFinishDuration, data.reloadExitDuration, data.maxAmmoCountInMagazine, OnPlayWeaponAnimation, OnReloadRequested);
    }
}