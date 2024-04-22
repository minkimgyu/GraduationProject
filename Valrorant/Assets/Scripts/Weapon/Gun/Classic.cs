using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

public class Classic : Gun
{
    [SerializeField] protected float _mainShootInterval;
    [SerializeField] protected float _subShootInterval;

    [SerializeField] protected int _mainFireCnt;
    [SerializeField] protected int _subFireCnt;

    [SerializeField] protected float _recoveryDuration;

    [SerializeField] protected float _penetratePower;

    [SerializeField] int _subActionPelletCount;
    [SerializeField] float _subActionSpreadOffset;

    [SerializeField] WeightApplier _mainWeightApplier;
    [SerializeField] WeightApplier _subWeightApplier;

    [SerializeField] protected float _displacementSpreadMultiplyRatio = 1f;


    [SerializeField]
    int _subAttackBulletCounts;

    [SerializeField]
    float _spreadOffset;

    [SerializeField]
    float _mainActionDelay;

    [SerializeField]
    float _subActionDelay;

    [SerializeField]
    float _bulletSpreadPowerRatio;

    [SerializeField]
    float _bulletSpreadPowerDecreaseRatio;

    Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> _damageDictionary = new Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]>()
    {
        { DistanceAreaData.HitArea.Head, new DistanceAreaData[]{ new DistanceAreaData(0, 30, 78), new DistanceAreaData(30, 50, 66) } },
        { DistanceAreaData.HitArea.Body, new DistanceAreaData[]{ new DistanceAreaData(0, 30, 26), new DistanceAreaData(30, 50, 22) } },
        { DistanceAreaData.HitArea.Leg, new DistanceAreaData[]{ new DistanceAreaData(0, 30, 22), new DistanceAreaData(30, 50, 18) } },
    };

    public override void Initialize(ClassicData data, RecoilRangeData mainRangeData, RecoilRangeData subRangeData)
    {
        _ammoCountsInMagazine = data.maxAmmoCountInMagazine;
        _ammoCountsInPossession = data.maxAmmoCountsInPossession;

        _reloadFinishDuration = data.reloadFinishDuration;
        _reloadExitDuration = data.reloadExitDuration;

        _eventStrategies[EventType.Main] = new ManualEvent(EventType.Main, _mainShootInterval, OnEventStart, OnEventUpdate, OnEventEnd, OnAction);
        _eventStrategies[EventType.Sub] = new ManualEvent(EventType.Sub, _subShootInterval, OnEventStart, OnEventUpdate, OnEventEnd, OnAction);

        _actionStrategies[EventType.Main] = new SingleProjectileAttackWithWeight(_weaponName, _range, _targetLayer, _mainFireCnt,
            _penetratePower, _displacementSpreadMultiplyRatio, _mainWeightApplier, _damageDictionary, OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
            SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest);


        _actionStrategies[EventType.Sub] = new ScatterProjectileAttackWithWeight(_weaponName, _range, _targetLayer, _subFireCnt,
            _penetratePower, _displacementSpreadMultiplyRatio, _subActionPelletCount, _subActionSpreadOffset, _subWeightApplier, _damageDictionary,
            OnPlayWeaponAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount, SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest);



        _recoilStrategies[EventType.Main] = new ManualRecoilGenerator(_mainShootInterval, _recoveryDuration, mainRangeData);
        _recoilStrategies[EventType.Sub] = new ManualRecoilGenerator(_subShootInterval, _recoveryDuration, subRangeData);


        _reloadStrategy = new MagazineReload(_weaponName, _reloadFinishDuration, _reloadExitDuration, _maxAmmoCountInMagazine, OnPlayWeaponAnimation, OnReloadRequested);
    }
}