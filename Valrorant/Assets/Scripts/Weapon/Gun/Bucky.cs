using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

[System.Serializable]
public class Bucky : NoVariationGun
{
    [SerializeField]
    float _mainActionDelay;

    [SerializeField]
    int _mainActionBulletCountInOneShoot;

    [SerializeField]
    int _subActionBulletCountInOneShoot;

    [SerializeField]
    int _pelletCount;

    [SerializeField]
    float _subActionDelay;

    [SerializeField]
    float _spreadOffset;

    [SerializeField]
    float _bulletSpreadPowerDecreaseRatio;

    [SerializeField]
    float _reloadBeforeDuration;


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

    public override void Initialize(GameObject player, GameObject armMesh, Transform cam, Animator ownerAnimator)
    {
        base.Initialize(player, armMesh, cam, ownerAnimator);

        Vector3 frontPosition = new Vector3(0, 0, _findRange);

        // ���⿡ Action �����ؼ� �Ѿ��� �Ҹ�Ǵ� �κ��� �����غ���
        _mainEventStrategy = new ManualAction(_mainActionDelay);
        _subEventStrategy = new ManualAction(_subActionDelay);

        _mainActionStrategy = new ScatterProjectileAttack(_camTransform, _range, _targetLayer, _mainActionBulletCountInOneShoot, ownerAnimator, _animator, _muzzleFlash, false,
            _emptyCartridgeSpawner, true, _weaponName.ToString(), _muzzle, _penetratePower, _trajectoryLineEffect, _spreadOffset, _pelletCount, _bulletSpreadPowerDecreaseRatio, 
            _damageDictionary, OnGenerateNoiseRequest);

        // ���⸦ ���� ���, �����ؾ���
        _subActionStrategy = new SingleAndExplosionScatterAttackCombination(_camTransform, _range, _targetLayer, ownerAnimator, _animator, _muzzleFlash, false, 
            _emptyCartridgeSpawner, false, _weaponName.ToString(), _muzzle, _trajectoryLineEffect, _findRange, _subActionSinglePenetratePower, _subActionBulletCountInOneShoot, 
            _subSingleActionBulletSpreadPowerDecreaseRatio, _damageDictionary, frontPosition, _explosionEffectName, _subActionScatterPenetratePower, _subActionSpreadOffset, 
            _subActionPelletCount, _subScatterActionBulletSpreadPowerDecreaseRatio, _subSingleAttackDamageDictionary, OnGenerateNoiseRequest);

        RecoilStorage storage = GameObject.FindWithTag("RecoilStorage").GetComponent<RecoilStorage>();
        RecoilRangeData mainRecoilData = storage.OnRecoilDataSendRequested<RecoilRangeData>(_weaponName, EventCallPart.Left);
        RecoilRangeData subRecoilData = storage.OnRecoilDataSendRequested<RecoilRangeData>(_weaponName, EventCallPart.Right);

        _mainRecoilStrategy = new ManualRecoilGenerator(_mainActionDelay, mainRecoilData);
        _subRecoilStrategy = new ManualRecoilGenerator(_subActionDelay, subRecoilData);

        _reloadStrategy = new RoundByRoundReload(_reloadBeforeDuration, _reloadFinishTime, _reloadExitTime, _weaponName.ToString(), _maxAmmoCountInMagazine, _animator, _ownerAnimator, OnReloadRequested);

        LinkEvent(player);
    }
}