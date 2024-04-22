using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;
using System;

public class Guardian : VariationGun
{
    [SerializeField] protected float _shootInterval;

    [SerializeField] protected float _zoomDuration;

    [SerializeField] protected int _fireAmmoCnt;

    [SerializeField] protected float _penetratePower;

    [SerializeField] protected float _displacementSpreadMultiplyRatio = 1;

    [SerializeField] WeightApplier _weightApplier;


    [SerializeField] protected Vector3 _cameraPositionWhenZoom;

    [SerializeField] protected float _normalFieldOfView;

    [SerializeField] protected float _zoomFieldOfView;


    [SerializeField] protected float _recoveryDuration;

    [SerializeField] protected float _recoilRatioWhenZoomOut = 1.0f;


    protected Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> _damageDictionary;

    //protected override void AddStrategies()
    //{
    //    //_eventStrategies[BaseStrategy.Type.Main] = new AutoEvent(BaseStrategy.Type.Main, _shootInterval);
    //    //_eventStrategies[BaseStrategy.Type.Sub] = new ManualEvent(BaseStrategy.Type.Sub, _zoomDuration);

    //    //_actionStrategies[BaseStrategy.Type.Main] = new SingleProjectileAttackWithWeight(BaseStrategy.Type.Main, _weaponName, _range, _targetLayer, _fireAmmoCnt,
    //    //    _penetratePower, _displacementSpreadMultiplyRatio, _weightApplier, _damageDictionary, OnPlayOwnerAnimation, ReturnMuzzlePos, ReturnLeftAmmoCount, DecreaseAmmoCount,
    //    //    SpawnMuzzleFlashEffect, SpawnEmptyCartridge, OnGenerateNoiseRequest);
    //    //_actionStrategies[BaseStrategy.Type.Sub] = new ZoomStrategy(BaseStrategy.Type.Sub, _cameraPositionWhenZoom, _zoomDuration, _normalFieldOfView, _zoomFieldOfView, OnZoomRequested);



    //    //_recoilStrategies[BaseStrategy.Type.Main] = new NoRecoilGenerator(BaseStrategy.Type.Main); // --> 수정해주기
    //    //_recoilStrategies[BaseStrategy.Type.Sub] = new NoRecoilGenerator(BaseStrategy.Type.Sub);

    //    //_reloadStrategy = new MagazineReload(_weaponName, _reloadFinishDuration, _reloadExitDuration, _maxAmmoCountInMagazine, OnPlayWeaponAnimation, OnPlayOwnerAnimation, OnReloadRequested);

    //    //_storedMainRecoilWhenZoomIn = new ManualRecoilGenerator(BaseStrategy.Type.Main, _shootInterval, _recoveryDuration, new RecoilRangeData());
    //    //_storedMainRecoilWhenZoomOut = new AutoRecoilGenerator(BaseStrategy.Type.Main, _shootInterval, _recoveryDuration, _recoilRatioWhenZoomOut, new RecoilMapData());
    //}
}
