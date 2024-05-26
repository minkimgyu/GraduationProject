using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

public class Bucky : Gun
{
    public override void Initialize(BuckyData data, RecoilRangeData mainRangeData, RecoilRangeData subRangeData)
    {
        _maxAmmoCountInMagazine = data.maxAmmoCountInMagazine;
        _maxAmmoCountsInPossession = data.maxAmmoCountsInPossession;

        _ammoCountsInMagazine = _maxAmmoCountInMagazine;
        _ammoCountsInPossession = _maxAmmoCountsInPossession;

        //���⿡ Action �����ؼ� �Ѿ��� �Ҹ�Ǵ� �κ��� �����غ���
        _eventStrategies[EventType.Main] = new ManualEvent(EventType.Main, data.mainFireInterval, OnEventStart, OnEventUpdate, OnEventEnd, OnAction);
        _eventStrategies[EventType.Sub] = new ManualEvent(EventType.Sub, data.subFireInterval, OnEventStart, OnEventUpdate, OnEventEnd, OnAction);


        _actionStrategies[EventType.Main] = new ScatterProjectileAttack(data.weaponName, data.range, _targetLayer, data.mainFireCnt,
            data.penetratePower, data.bulletSpreadPowerDecreaseRatio, data.pelletCount, data.spreadOffset, data.damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest, PlaySFX);

        // ���⸦ ���� ���, �����ؾ���
        _actionStrategies[EventType.Sub] = new SingleAndExplosionScatterAttackCombination(

            data.weaponName, data.range, _targetLayer, data.mainFireCnt, data.subActionSinglePenetratePower, data.subScatterActionBulletSpreadPowerDecreaseRatio, data.subFireCnt,
            data.subActionScatterPenetratePower, data.subSingleActionBulletSpreadPowerDecreaseRatio, data.pelletCount, data.spreadOffset, data.frontDistance, data.explosionEffectName, data.damageDictionary,
            data.subSingleAttackDamageDictionary, data.findRange, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest, PlaySFX);

        _recoilStrategies[EventType.Main] = new ManualRecoilGenerator(data.mainFireInterval, data.recoveryDuration, mainRangeData);
        _recoilStrategies[EventType.Sub] = new ManualRecoilGenerator(data.subFireInterval, data.recoveryDuration, subRangeData);

        _reloadStrategy = new RoundByRoundReload(data.weaponName, data.reloadBeforeDuration, data.reloadFinishDuration, data.reloadExitDuration, data.maxAmmoCountInMagazine, OnPlayWeaponAnimation, OnReloadRequested, PlaySFX);
    }
}