using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

public class Stinger : VariationGun
{
    public override void Initialize(StingerData data, RecoilMapData mainMapData, RecoilRangeData subRangeData)
    {
        _ammoCountsInMagazine = data.maxAmmoCountInMagazine;
        _ammoCountsInPossession = data.maxAmmoCountsInPossession;

        _eventStorage.Add(new(EventType.Main, Conditon.ZoomIn),
            new BurstEvent(EventType.Main, data.burstFireInterval, data.burstFireCntInOneAction, OnEventStart, OnEventUpdate, OnEventEnd, OnAction));

        _eventStorage.Add(new(EventType.Main, Conditon.ZoomOut),
            new AutoEvent(EventType.Main, data.autoFireInterval, OnEventStart, OnEventUpdate, OnEventEnd, OnAction));

        _eventStorage.Add(new(EventType.Sub, Conditon.Both),
            new ManualEvent(EventType.Sub, data.zoomDelay, OnEventStart, OnEventUpdate, OnEventEnd, OnAction));


        _actionStorage.Add(new(EventType.Main, Conditon.ZoomIn),
            new SingleProjectileAttackWithWeight(data.weaponName, data.range, _targetLayer, data.mainFireCnt,
            data.penetratePower, data.bulletSpreadPowerDecreaseRatio, data.weightApplier, data.damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest));

        _actionStorage.Add(new(EventType.Main, Conditon.ZoomOut),
             new SingleProjectileAttack(data.weaponName, data.range, _targetLayer, data.mainFireCnt,
             data.penetratePower, data.bulletSpreadPowerDecreaseRatio, data.damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
             SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest));


        _actionStorage.Add(new(EventType.Sub, Conditon.Both),
            new ZoomStrategy(data.zoomCameraPosition.V3, data.zoomDuration, data.normalFieldOfView, data.zoomFieldOfView, OnZoomRequested));

        _recoilStorage.Add(new(EventType.Main, Conditon.ZoomOut),
            new AutoRecoilGenerator(data.autoFireInterval, data.recoveryDuration, data.recoilRatio, mainMapData));

        _recoilStorage.Add(new(EventType.Main, Conditon.ZoomIn),
            new BurstRecoilGenerator(data.burstFireInterval, data.recoveryDuration, subRangeData));

        _recoilStorage.Add(new(EventType.Sub, Conditon.Both), new NoRecoilGenerator());


        _reloadStrategy = new MagazineReload(data.weaponName, data.reloadFinishDuration, data.reloadExitDuration, data.maxAmmoCountInMagazine, OnPlayWeaponAnimation, OnReloadRequested);
    }
}
