using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;
using UnityEngine.UI;

public class Operator : VariationGun
{
    public override void Initialize(OperatorData data, RecoilRangeData mainRangeData, RecoilRangeData subRangeData)
    {
        _maxAmmoCountInMagazine = data.maxAmmoCountInMagazine;
        _maxAmmoCountsInPossession = data.maxAmmoCountsInPossession;

        _ammoCountsInMagazine = _maxAmmoCountInMagazine;
        _ammoCountsInPossession = _maxAmmoCountsInPossession;

        _eventStorage.Add(new(EventType.Main, Conditon.ZoomIn),
            new ManualEvent(EventType.Main, data.mainActionDelayWhenZoomIn, OnEventStart, OnEventUpdate, OnEventEnd, OnAction));

        _eventStorage.Add(new(EventType.Main, Conditon.ZoomOut),
            new ManualEvent(EventType.Main, data.mainActionDelayWhenZoomOut, OnEventStart, OnEventUpdate, OnEventEnd, OnAction));

        _eventStorage.Add(new(EventType.Sub, Conditon.Both),
            new ManualEvent(EventType.Sub, data.subActionDelay, OnEventStart, OnEventUpdate, OnEventEnd, OnAction));


        _actionStorage.Add(new(EventType.Main, Conditon.ZoomIn),
            new SingleProjectileAttack(data.weaponName, data.range, _targetLayer, data.fireCnt,
            data.penetratePower, data.mainActionbulletSpreadPowerRatio, data.damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest, PlaySFX));

        _actionStorage.Add(new(EventType.Main, Conditon.ZoomOut),
            new SingleProjectileAttackWithWeight(data.weaponName, data.range, _targetLayer, data.fireCnt,
            data.penetratePower, data.mainActionbulletSpreadPowerRatio, data.mainWeightApplier, data.damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest, PlaySFX));

        _actionStorage.Add(new(EventType.Sub, Conditon.Both),
            new DoubleZoomStrategy(data.zoomCameraPosition.V3, data.zoomDuration, data.normalFieldOfView, data.zoomFieldOfView, data.doubleZoomFieldOfView, OnZoomRequested, PlaySFX));

        _recoilStorage.Add(new(EventType.Main, Conditon.ZoomIn),
            new ManualRecoilGenerator(data.mainActionDelayWhenZoomIn, data.recoveryDuration, mainRangeData));

        _recoilStorage.Add(new(EventType.Main, Conditon.ZoomOut),
            new ManualRecoilGenerator(data.mainActionDelayWhenZoomOut, data.recoveryDuration, subRangeData));

        _recoilStorage.Add(new(EventType.Sub, Conditon.Both), new NoRecoilGenerator());

        _reloadStrategy = new MagazineReload(data.weaponName, data.reloadFinishDuration, data.reloadExitDuration, data.maxAmmoCountInMagazine, OnPlayWeaponAnimation, OnReloadRequested, PlaySFX);
    }
}
