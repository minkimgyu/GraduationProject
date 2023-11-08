using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

[System.Serializable]
public class Guardian : RecoilVariationGun
{
    [SerializeField]
    protected float _mainActionDelay;

    [SerializeField]
    protected float _recoilRatioWhenZoomIn = 1.0f;

    [SerializeField]
    protected float _recoilRatioWhenZoomOut = 0.5f;


    [SerializeField]
    protected float _subActionDelay;

    [SerializeField]
    protected GameObject _scope;

    /// <summary>
    /// 움직임에 따라 변경되는 값
    /// </summary>
    [SerializeField]
    protected float _displacementSpreadMultiplyRatio = 1;

    [SerializeField]
    protected float _zoomDuration = 0.5f;

    [SerializeField]
    protected float _delayUntilScopeActivated = 0.1f;

    [SerializeField]
    protected float _normalFieldOfView;

    [SerializeField]
    protected float _zoomFieldOfView;

    [SerializeField]
    protected Vector3 _cameraPositionWhenZoom;

    [SerializeField]
    protected RecoilRange _recoilRange;

    [SerializeField]
    protected WeightApplier _weightApplier;

    protected Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> _damageDictionary;

    public override void Initialize(GameObject player, GameObject armMesh, Transform cam, Animator ownerAnimator)
    {
        //base.Initialize(player, armMesh, cam, ownerAnimator);

        //_mainActionStrategy = new AutoAttackAction(_mainActionDelay);
        //_subActionStrategy = new ManualAction(_subActionDelay);

        //_mainResultStrategy = new SingleProjectileAttackWithWeight(_camTransform, _range, _targetLayer, ownerAnimator, _animator, _muzzleFlash, false,
        //    _emptyCartridgeSpawner, true, _weaponName.ToString(), _muzzle, _penetratePower, _trajectoryLineEffect, _displacementSpreadMultiplyRatio, _damageDictionary, _weightApplier);

        //_subResultStrategy = new ZoomStrategy(_scope, _cameraPositionWhenZoom, _zoomDuration, _delayUntilScopeActivated, _normalFieldOfView, _zoomFieldOfView, OnZoomEventCall);

        //_storedMainRecoilWhenZoomIn = new ManualRecoilGenerator(_mainActionDelay, _recoilRange);
        //_storedMainRecoilWhenZoomOut = new AutoRecoilGenerator(_mainActionDelayWhenZoomIn, _recoilRatioWhenZoomOut, _recoilMap);


        //_subRecoilStrategy = new NoRecoilGenerator();

        //_reloadStrategy = new MagazineReload(_reloadFinishTime, _reloadExitTime, _weaponName.ToString(), _maxAmmoCountInMagazine, _animator, _ownerAnimator, OnReloadRequested);

        //LinkEvent(player);
    }
}
