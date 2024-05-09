using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;
using System;

abstract public class AutomaticGun : VariationGun
{
    public override void Initialize(AutomaticGunData data, RecoilMapData recoilData)
    {
        _ammoCountsInMagazine = data.maxAmmoCountInMagazine;
        _ammoCountsInPossession = data.maxAmmoCountsInPossession;

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
            new ZoomStrategy(data.cameraPositionWhenZoom.V3, data.zoomDuration, data.normalFieldOfView, data.zoomFieldOfView, OnZoomRequested));


        _recoilStorage.Add(new(EventType.Main, Conditon.ZoomIn),
            new AutoRecoilGenerator(data.fireIntervalWhenZoomIn, data.recoveryDuration, data.recoilRatioWhenZoomIn, recoilData));

        _recoilStorage.Add(new(EventType.Main, Conditon.ZoomOut),
            new AutoRecoilGenerator(data.fireIntervalWhenZoomIn, data.recoveryDuration, data.recoilRatioWhenZoomOut, recoilData));

        _recoilStorage.Add(new(EventType.Sub, Conditon.Both), new NoRecoilGenerator());

        _reloadStrategy = new MagazineReload(data.weaponName, data.reloadFinishDuration, data.reloadExitDuration, data.maxAmmoCountInMagazine, OnPlayWeaponAnimation, OnReloadRequested);
    }
}