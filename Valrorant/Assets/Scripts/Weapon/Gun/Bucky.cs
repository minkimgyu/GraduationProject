using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

public class Bucky : VariationGun
{
    [SerializeField] float _mainFireInterval;
    [SerializeField] float _subFireInterval;


    [SerializeField] protected int _mainFireCnt;
    [SerializeField] protected int _subFireCnt;

    [SerializeField] protected float _recoveryDuration;

    [SerializeField] protected float _penetratePower;

    [SerializeField] int _pelletCount;
    [SerializeField] float _spreadOffset;


    [SerializeField]
    float _bulletSpreadPowerDecreaseRatio;

    [SerializeField] float _reloadBeforeDuration;


    [SerializeField] float _frontDistance;


    [SerializeField]
    string _explosionEffectName;

    [SerializeField]
    int _subActionPelletCount;

    [SerializeField]
    float _subScatterActionBulletSpreadPowerDecreaseRatio;

    [SerializeField]
    float _subSingleActionBulletSpreadPowerDecreaseRatio;

    [SerializeField]
    float _subActionSpreadOffset;

    [SerializeField]
    float _subActionSinglePenetratePower;

    [SerializeField]
    float _subActionScatterPenetratePower;

    [SerializeField]
    float _findRange;

    Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> _damageDictionary = new Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]>()
    {
        { DistanceAreaData.HitArea.Head, new DistanceAreaData[]{ new DistanceAreaData(0, 8, 40), new DistanceAreaData(8, 12, 26), new DistanceAreaData(12, 50, 18) } },
        { DistanceAreaData.HitArea.Body, new DistanceAreaData[]{ new DistanceAreaData(0, 8, 20), new DistanceAreaData(8, 12, 13), new DistanceAreaData(12, 50, 9) } },
        { DistanceAreaData.HitArea.Leg, new DistanceAreaData[]{ new DistanceAreaData(0, 8, 17), new DistanceAreaData(8, 12, 11), new DistanceAreaData(12, 50, 7) } },
    };

    Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> _subSingleAttackDamageDictionary = new Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]>()
    {
        { DistanceAreaData.HitArea.Head, new DistanceAreaData[]{ new DistanceAreaData(0, 50, 20) } },
        { DistanceAreaData.HitArea.Body, new DistanceAreaData[]{ new DistanceAreaData(0, 50, 20) } },
        { DistanceAreaData.HitArea.Leg, new DistanceAreaData[]{ new DistanceAreaData(0, 50, 20) } },
    };

    public override void Initialize(BuckyData data, RecoilRangeData mainRangeData, RecoilRangeData subRangeData)
    {
        _ammoCountsInMagazine = data.maxAmmoCountInMagazine;
        _ammoCountsInPossession = data.maxAmmoCountsInPossession;

        _reloadFinishDuration = data.reloadFinishDuration;
        _reloadExitDuration = data.reloadExitDuration;

        //여기에 Action 연결해서 총알이 소모되는 부분을 구현해보자
        _eventStrategies[EventType.Main] = new ManualEvent(EventType.Main, _mainFireInterval, OnEventStart, OnEventUpdate, OnEventEnd, OnAction);
        _eventStrategies[EventType.Sub] = new ManualEvent(EventType.Sub, _subFireInterval, OnEventStart, OnEventUpdate, OnEventEnd, OnAction);


        _actionStrategies[EventType.Main] = new ScatterProjectileAttack(_weaponName, _range, _targetLayer, _mainFireCnt,
            _penetratePower, _bulletSpreadPowerDecreaseRatio, _pelletCount, _spreadOffset, _damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest);

        // 무기를 버릴 경우, 제거해야함
        _actionStrategies[EventType.Sub] = new SingleAndExplosionScatterAttackCombination(

            _weaponName, _range, _targetLayer, _mainFireCnt, _subActionSinglePenetratePower, _subScatterActionBulletSpreadPowerDecreaseRatio, _subFireCnt,
            _subActionScatterPenetratePower, _subSingleActionBulletSpreadPowerDecreaseRatio, _pelletCount, _spreadOffset, _frontDistance, _explosionEffectName, _damageDictionary,
            _subSingleAttackDamageDictionary, _findRange, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest);

        _recoilStrategies[EventType.Main] = new ManualRecoilGenerator(_mainFireInterval, _recoveryDuration, mainRangeData);
        _recoilStrategies[EventType.Sub] = new ManualRecoilGenerator(_subFireInterval, _recoveryDuration, subRangeData);

        _reloadStrategy = new RoundByRoundReload(_weaponName, _reloadBeforeDuration, _reloadFinishDuration, _reloadExitDuration, _maxAmmoCountInMagazine, OnPlayWeaponAnimation, OnReloadRequested);
    }
}