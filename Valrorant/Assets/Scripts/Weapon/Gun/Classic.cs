using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

public class Classic : Gun
{
    public override void Initialize(ClassicData data, RecoilRangeData mainRangeData, RecoilRangeData subRangeData)
    {
        _ammoCountsInMagazine = data.maxAmmoCountInMagazine;
        _ammoCountsInPossession = data.maxAmmoCountsInPossession;

        _eventStrategies[EventType.Main] = new ManualEvent(EventType.Main, data.mainShootInterval, OnEventStart, OnEventUpdate, OnEventEnd, OnAction);
        _eventStrategies[EventType.Sub] = new ManualEvent(EventType.Sub, data.subShootInterval, OnEventStart, OnEventUpdate, OnEventEnd, OnAction);

        

        _actionStrategies[EventType.Main] = new SingleProjectileAttackWithWeight(data.weaponName, data.range, _targetLayer, data.mainFireCnt,
            data.penetratePower, data.displacementSpreadMultiplyRatio, data.mainWeightApplier, data.damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest);


        _actionStrategies[EventType.Sub] = new ScatterProjectileAttackWithWeight(data.weaponName, data.range, _targetLayer, data.subFireCnt,
            data.penetratePower, data.displacementSpreadMultiplyRatio, data.subFireCnt, data.subActionSpreadOffset, data.subWeightApplier, data.damageDictionary,
            OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount, SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest);



        _recoilStrategies[EventType.Main] = new ManualRecoilGenerator(data.mainShootInterval, data.recoveryDuration, mainRangeData);
        _recoilStrategies[EventType.Sub] = new ManualRecoilGenerator(data.subShootInterval, data.recoveryDuration, subRangeData);


        _reloadStrategy = new MagazineReload(_weaponName, data.reloadFinishDuration, data.reloadExitDuration, _maxAmmoCountInMagazine, OnPlayWeaponAnimation, OnReloadRequested);
    }
}