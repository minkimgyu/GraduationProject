using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judge : Gun
{
    public override void Initialize(JudgeData data, RecoilRangeData mainRangeData, RecoilRangeData subRangeData)
    {
        _ammoCountsInMagazine = data.maxAmmoCountInMagazine;
        _ammoCountsInPossession = data.maxAmmoCountsInPossession;

        //���⿡ Action �����ؼ� �Ѿ��� �Ҹ�Ǵ� �κ��� �����غ���
        _eventStrategies[EventType.Main] = new ManualEvent(EventType.Main, data.mainFireInterval, OnEventStart, OnEventUpdate, OnEventEnd, OnAction);
        _eventStrategies[EventType.Sub] = new ManualEvent(EventType.Sub, data.subFireInterval, OnEventStart, OnEventUpdate, OnEventEnd, OnAction);


        _actionStrategies[EventType.Main] = new ScatterProjectileAttack(data.weaponName, data.range, _targetLayer, data.mainFireCnt,
            data.penetratePower, data.bulletSpreadPowerDecreaseRatio, data.pelletCount, data.spreadOffset, data.damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest);

        // ���⸦ ���� ���, �����ؾ���
        _actionStrategies[EventType.Sub] = new SingleAndExplosionScatterAttackCombination(

            data.weaponName, data.range, _targetLayer, data.mainFireCnt, data.subActionSinglePenetratePower, data.subScatterActionBulletSpreadPowerDecreaseRatio, data.subFireCnt,
            data.subActionScatterPenetratePower, data.subSingleActionBulletSpreadPowerDecreaseRatio, data.pelletCount, data.spreadOffset, data.frontDistance, data.explosionEffectName, data.damageDictionary,
            data.subSingleAttackDamageDictionary, data.findRange, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest);

        _recoilStrategies[EventType.Main] = new ManualRecoilGenerator(data.mainFireInterval, data.recoveryDuration, mainRangeData);
        _recoilStrategies[EventType.Sub] = new ManualRecoilGenerator(data.subFireInterval, data.recoveryDuration, subRangeData);

        _reloadStrategy = new MagazineReload(data.weaponName, data.reloadFinishDuration, data.reloadExitDuration, data.maxAmmoCountInMagazine, OnPlayWeaponAnimation, OnReloadRequested);
    }
}
