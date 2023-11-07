using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

[System.Serializable]
public class Classic : NoVariationGun
{
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

    [SerializeField]
    RecoilRange _mainActionRecoilRange;

    [SerializeField]
    RecoilRange _subActionRecoilRange;

    [SerializeField]
    WeightApplier _mainWeightApplier;

    [SerializeField]
    WeightApplier _subWeightApplier;

    Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> _attackDamageDictionary = new Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]>()
    {
        { DistanceAreaData.HitArea.Head, new DistanceAreaData[]{ new DistanceAreaData(0, 30, 78), new DistanceAreaData(30, 50, 66) } },
        { DistanceAreaData.HitArea.Body, new DistanceAreaData[]{ new DistanceAreaData(0, 30, 26), new DistanceAreaData(30, 50, 22) } },
        { DistanceAreaData.HitArea.Leg, new DistanceAreaData[]{ new DistanceAreaData(0, 30, 22), new DistanceAreaData(30, 50, 18) } },
    };

    public override void Initialize(GameObject player, GameObject armMesh, Transform cam, Animator ownerAnimator)
    {
        base.Initialize(player, armMesh, cam, ownerAnimator);

        _mainResultStrategy = new SingleProjectileAttackWithWeight(_camTransform, _range, _targetLayer, ownerAnimator,_animator, _muzzleFlash, false, _emptyCartridgeSpawner, 
            true, _weaponName.ToString(), _muzzle, _penetratePower, _trajectoryLineEffect, _bulletSpreadPowerDecreaseRatio, _attackDamageDictionary, _mainWeightApplier);

        _subResultStrategy = new ScatterProjectileAttackWithWeight(_camTransform, _range, _targetLayer, _subAttackBulletCounts, ownerAnimator, _animator, _muzzleFlash, false, _emptyCartridgeSpawner,
            false, _weaponName.ToString(), _muzzle, _penetratePower, _trajectoryLineEffect, _spreadOffset, _subAttackBulletCounts, _bulletSpreadPowerRatio, _attackDamageDictionary, _subWeightApplier);


        _mainActionStrategy = new ManualAttackAction(_mainActionDelay);
        _subActionStrategy = new ManualAttackAction(_subActionDelay);

        _mainRecoilStrategy = new ManualRecoilGenerator(_mainActionDelay, _mainActionRecoilRange);
        _subRecoilStrategy = new ManualRecoilGenerator(_mainActionDelay, _mainActionRecoilRange);

        _reloadStrategy = new MagazineReload(_reloadFinishTime, _reloadExitTime, _weaponName.ToString(), _maxAmmoCountInMagazine, _animator, _ownerAnimator, OnReloadRequested);

        LinkEvent(player);
    }
}