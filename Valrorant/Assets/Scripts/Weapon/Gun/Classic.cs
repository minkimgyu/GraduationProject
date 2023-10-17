using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;
using ObserverPattern;

public class Classic : Gun
{
    [SerializeField]
    int mainAttackBulletCount = 1;

    [SerializeField]
    int _projectileCounts;

    [SerializeField]
    float _spreadOffset;

    [SerializeField]
    float _mainActionDelay;

    [SerializeField]
    float _subActionDelay;

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

    public override void Initialize(GameObject player, Transform cam, Animator ownerAnimator)
    {
        base.Initialize(player, cam, ownerAnimator);

        _mainResult = new SingleProjectileAttackWithWeight(_camTransform, _range, _hitEffectName,
            _targetLayer, _muzzle, _penetratePower, _nonPenetrateHitEffect, _trajectoryLineEffect, _attackDamageDictionary, _mainWeightApplier);

        _subResult = new ScatterProjectileGunAttackWithWeight(_camTransform, _range, _hitEffectName,
            _targetLayer, _muzzle, _penetratePower, _nonPenetrateHitEffect, _trajectoryLineEffect, _projectileCounts, _spreadOffset, _attackDamageDictionary, _subWeightApplier);
       

         _mainAction = new ManualAttackAction(_mainActionDelay);
        _subAction = new ManualAttackAction(_subActionDelay);


        IObserver<Vector2, Vector2, Vector2> recoilObserver = player.GetComponent<IObserver<Vector2, Vector2, Vector2>>();


        RecoilStrategy mainRecoilGenerator = new ManualRecoilGenerator(_mainActionDelay, _mainActionRecoilRange.RecoilRecoverDuration, _mainActionRecoilRange);
        mainRecoilGenerator.AddObserver(recoilObserver);

        RecoilStrategy subRecoilGenerator = new ManualRecoilGenerator(_mainActionDelay, _mainActionRecoilRange.RecoilRecoverDuration, _mainActionRecoilRange);
        subRecoilGenerator.AddObserver(recoilObserver);

        _mainRecoilGenerator = mainRecoilGenerator;
        _subRecoilGenerator = subRecoilGenerator;

        LinkActionStrategy();
    }

    protected override void ChainMainActionStartEvent()
    {
        if (_bulletCountInMagazine <= 0) return;

        PlayMainActionAnimation();

        OnAttack();

        // 여기서 총알 감소 부분 추가해주기
        _bulletCountInMagazine -= mainAttackBulletCount; // 1발 발사

        _mainResult.Do(_receivedBulletSpreadPower);

        _mainRecoilGenerator.CreateRecoil();
    }

    protected override void ChainSubActionStartEvent()
    {
        if (_bulletCountInMagazine <= 0) return;

        PlaySubActionAnimation();

        OnAttack();

        // 여기서 총알 감소 부분 추가해주기
        _bulletCountInMagazine -= _projectileCounts; // 3발 발사

        _subResult.Do(_receivedBulletSpreadPower);

        _subRecoilGenerator.CreateRecoil();
    }
}
