using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;
using UnityEngine.UI;

[System.Serializable]
public class Operator : AllVariationGun
{
    [SerializeField]
    float _mainActionDelayWhenZoomIn;

    [SerializeField]
    float _mainActionDelayWhenZoomOut;

    [SerializeField]
    float _subActionDelay;

    [SerializeField]
    float _mainActionbulletSpreadPowerRatio;

    GameObject _scope;

    [SerializeField]
    float _meshDisableDelay;

    [SerializeField]
    float _zoomDuration;

    [SerializeField]
    float _scopeOnDelay;

    [SerializeField]
    float _normalFieldOfView;

    [SerializeField]
    float _zoomFieldOfView;

    [SerializeField]
    float _doubleZoomFieldOfView;

    [SerializeField]
    Vector3 _zoomCameraPosition;

    [SerializeField]
    GameObject _gunMesh;

    [SerializeField]
    WeightApplier _mainWeightApplier;

    Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> _attackDamageDictionary = new Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]>()
    {
        { DistanceAreaData.HitArea.Head, new DistanceAreaData[]{ new DistanceAreaData(0, 50, 255) } },
        { DistanceAreaData.HitArea.Body, new DistanceAreaData[]{ new DistanceAreaData(0, 50, 150) } },
        { DistanceAreaData.HitArea.Leg, new DistanceAreaData[]{ new DistanceAreaData(0, 50, 120) } },
    };

    public override void Initialize(GameObject player, GameObject armMesh, Transform cam, Animator ownerAnimator)
    {
        base.Initialize(player, armMesh, cam, ownerAnimator);

        _storedMainActionWhenZoomIn = new ManualAttackAction(_mainActionDelayWhenZoomIn);
        _storedMainActionWhenZoomOut = new ManualAttackAction(_mainActionDelayWhenZoomOut);

        _subEventStrategy = new ManualAttackAction(_subActionDelay);

        _storedMainResultWhenZoomOut = new SingleProjectileAttackWithWeight(_camTransform, _range, _targetLayer, ownerAnimator, _animator, _muzzleFlash, false, _emptyCartridgeSpawner,
            true, _weaponName.ToString(), _muzzle, _penetratePower, _trajectoryLineEffect, _mainActionbulletSpreadPowerRatio, _attackDamageDictionary, OnGenerateNoiseRequest, _mainWeightApplier);

        _storedMainResultWhenZoomIn = new SingleProjectileAttack(_camTransform, _range, _targetLayer, ownerAnimator, _animator, _muzzleFlash, false, _emptyCartridgeSpawner,
            true, _weaponName.ToString(), _muzzle, _penetratePower, _trajectoryLineEffect, _mainActionbulletSpreadPowerRatio, _attackDamageDictionary, OnGenerateNoiseRequest);


        GameObject scopeContainer = GameObject.FindWithTag("ScopeContainer");
        _scope = scopeContainer.GetComponent<ScopeContainer>().ReturnScope(); // 찾아서 넘겨줌

        _subActionStrategy = new DoubleZoomStrategy(_scope, armMesh, _gunMesh, _zoomCameraPosition, _zoomDuration, _scopeOnDelay, _normalFieldOfView, _zoomFieldOfView, _doubleZoomFieldOfView, OnZoomEventCall, _meshDisableDelay);

        RecoilStorage storage = GameObject.FindWithTag("RecoilStorage").GetComponent<RecoilStorage>();
        RecoilRangeData mainRecoilData = storage.OnRecoilDataSendRequested<RecoilRangeData>(_weaponName, EventCallPart.Left);
        RecoilRangeData subRecoilData = storage.OnRecoilDataSendRequested<RecoilRangeData>(_weaponName, EventCallPart.Right);

        _storedMainRecoilWhenZoomIn = new ManualRecoilGenerator(_mainActionDelayWhenZoomIn, mainRecoilData);
        _storedMainRecoilWhenZoomOut = new ManualRecoilGenerator(_mainActionDelayWhenZoomOut, subRecoilData);

        _subRecoilStrategy = new NoRecoilGenerator();

        _reloadStrategy = new MagazineReload(_reloadFinishTime, _reloadExitTime, _weaponName.ToString(), _maxAmmoCountInMagazine, _animator, _ownerAnimator, OnReloadRequested);

        LinkEvent(player);
    }
}
