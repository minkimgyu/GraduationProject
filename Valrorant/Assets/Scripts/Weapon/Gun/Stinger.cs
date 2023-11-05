using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

[System.Serializable]
public class Stinger : AllVariationGun
{
    [SerializeField]
    float _autoAttackActionDelay;

    [SerializeField]
    float _burstAttackActionDelay;

    [SerializeField]
    float _burstAttackRecoverDuration; // �̰� �� �� ������



    [SerializeField]
    float _subActionDelay;

    [SerializeField]
    GameObject _scope;

    [SerializeField]
    float _zoomDuration;

    [SerializeField]
    float _scopeOnDelay;

    [SerializeField]
    RecoilMap _recoilMap;

    [SerializeField]
    float _normalFieldOfView;

    [SerializeField]
    float _zoomFieldOfView;

    [SerializeField]
    Vector3 _zoomCameraPosition;

    Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> _damageDictionary = new Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]>()
    {
        { DistanceAreaData.HitArea.Head, new DistanceAreaData[]{ new DistanceAreaData(0, 50, 160) } },
        { DistanceAreaData.HitArea.Body, new DistanceAreaData[]{ new DistanceAreaData(0, 50, 40) } },
        { DistanceAreaData.HitArea.Leg, new DistanceAreaData[]{ new DistanceAreaData(0, 50, 34) } },
    };

    [SerializeField]
    WeightApplier _weightApplier;

    [SerializeField]
    RecoilRange _recoilRange;

    public override void Initialize(GameObject player, Transform cam, Animator ownerAnimator)
    {
        base.Initialize(player, cam, ownerAnimator);

        // ���⿡ Action �����ؼ� �Ѿ��� �Ҹ�Ǵ� �κ��� �����غ���
        _storedMainActionStrategyWhenZoomOut = new AutoAttackAction(_autoAttackActionDelay);
        _storedMainActionStrategyWhenZoomIn = new BurstAttackAction(_burstAttackActionDelay, _burstAttackRecoverDuration, 4);
        // �� ���� ����Ī�ؼ� �������

        _subActionStrategy = new ManualAction(_subActionDelay); // �� �׼�


        _storedMainResultStrategyWhenZoomOut = new SingleProjectileAttack(_camTransform, _range, _targetLayer, ownerAnimator, _animator, _muzzleFlash, false,
            _emptyCartridgeSpawner, true, _weaponName.ToString(), _muzzle, _penetratePower, _trajectoryLineEffect, _damageDictionary);

        _storedMainResultStrategyWhenZoomIn = new SingleProjectileAttackWithWeight(_camTransform, _range, _targetLayer, ownerAnimator, _animator, _muzzleFlash, false,
            _emptyCartridgeSpawner, true, _weaponName.ToString(), _muzzle, _penetratePower, _trajectoryLineEffect, _damageDictionary, _weightApplier);


        // ���⸦ ���� ���, �����ؾ���
        _subResultStrategy = new ZoomStrategy(_scope, _zoomCameraPosition, _zoomDuration, _scopeOnDelay, _normalFieldOfView, _zoomFieldOfView, OnZoomEventCall); // OnZoomRequested �Լ� �־ ������ ��

        _storedMainRecoilStrategyWhenZoomOut = new AutoRecoilGenerator(_autoAttackActionDelay, _recoilMap);
        _storedMainRecoilStrategyWhenZoomIn = new BurstRecoilGenerator(_burstAttackActionDelay, _recoilRange);

        _subRecoilStrategy = new NoRecoilGenerator();

        _reloadStrategy = new MagazineReload(_reloadFinishTime, _reloadExitTime, _weaponName.ToString(), _animator, _ownerAnimator, OnReloadRequested);
        LinkEvent(player); // �̰� ���ְ� �Ű������� �Ѱܼ� ��ũ��Ű�� ������� ����
    }
}
