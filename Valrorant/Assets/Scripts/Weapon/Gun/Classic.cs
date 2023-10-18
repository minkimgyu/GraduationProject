using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;
using ObserverPattern;

[System.Serializable]
public class Classic : Gun
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
            _targetLayer, _muzzle, _penetratePower, _nonPenetrateHitEffect, _trajectoryLineEffect, _spreadOffset, _attackDamageDictionary, _subWeightApplier);
       

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

    // ¼öÁ¤
    protected override void ChainMainActionStartEvent()
    {
        bool canFire = Fire(_mainResult, _mainRecoilGenerator);
        if(canFire) PlayMainActionAnimation();
    }

    protected override void ChainSubActionStartEvent()
    {
        bool canFire = Fire(_subResult, _subRecoilGenerator, _subAttackBulletCounts);
        if (canFire) PlaySubActionAnimation();
    }
}
