using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;
using System;

public class Guardian : VariationGun
{
    public override void Initialize(GuardianData data, RecoilRangeData recoilData)
    {
        _maxAmmoCountInMagazine = data.maxAmmoCountInMagazine;
        _maxAmmoCountsInPossession = data.maxAmmoCountsInPossession;

        _ammoCountsInMagazine = _maxAmmoCountInMagazine;
        _ammoCountsInPossession = _maxAmmoCountsInPossession;

        _eventStorage.Add(new(EventType.Main, Conditon.ZoomIn),
            new ManualEvent(EventType.Main, data.fireIntervalWhenZoomIn, OnEventStart, OnEventUpdate, OnEventEnd, OnAction));

        _eventStorage.Add(new(EventType.Main, Conditon.ZoomOut)
            , new ManualEvent(EventType.Main, data.fireIntervalWhenZoomOut, OnEventStart, OnEventUpdate, OnEventEnd, OnAction));

        _eventStorage.Add(new(EventType.Sub, Conditon.Both)
            , new ManualEvent(EventType.Sub, data.zoomDelay, OnEventStart, OnEventUpdate, OnEventEnd, OnAction));


        _actionStorage.Add(new(EventType.Main, Conditon.Both),
            new SingleProjectileAttackWithWeight(data.weaponName, data.range, _targetLayer, data.fireCnt,
            data.penetratePower, data.displacementSpreadMultiplyRatio, data.mainWeightApplier, data.damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest, PlaySFX));

        _actionStorage.Add(new(EventType.Sub, Conditon.Both),
            new ZoomStrategy(data.cameraPositionWhenZoom.V3, data.zoomDuration, data.normalFieldOfView, data.zoomFieldOfView, OnZoomRequested, PlaySFX));


        _recoilStorage.Add(new(EventType.Main, Conditon.ZoomIn),
            new ManualRecoilGenerator(data.fireIntervalWhenZoomIn, data.recoveryDuration, recoilData));

        _recoilStorage.Add(new(EventType.Main, Conditon.ZoomOut),
           new ManualRecoilGenerator(data.fireIntervalWhenZoomOut, data.recoveryDuration, recoilData));

        _recoilStorage.Add(new(EventType.Sub, Conditon.Both), new NoRecoilGenerator());

        _reloadStrategy = new MagazineReload(data.weaponName, data.reloadFinishDuration, data.reloadExitDuration, data.maxAmmoCountInMagazine, OnPlayWeaponAnimation, OnReloadRequested, PlaySFX);
    }
}
