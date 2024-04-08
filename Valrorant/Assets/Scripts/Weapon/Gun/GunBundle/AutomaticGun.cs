using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

abstract public class AutomaticGun : ActionAndRecoilVariationGun
{
    [SerializeField]
    protected float _mainActionDelayWhenZoomIn;

    [SerializeField]
    protected float _mainActionDelayWhenZoomOut;

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

    //[SerializeField]
    //protected RecoilMap _recoilMap;

    //protected BaseRecoilData _recoilData;

    [SerializeField]
    protected float _normalFieldOfView;

    [SerializeField]
    protected float _zoomFieldOfView;

    [SerializeField]
    protected Vector3 _cameraPositionWhenZoom;

    protected Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> _damageDictionary;

    public override void Initialize(GameObject player, GameObject armMesh, Transform cam, Animator ownerAnimator)
    {
        base.Initialize(player, armMesh, cam, ownerAnimator);

        _storedMainActionWhenZoomIn = new AutoAttackAction(_mainActionDelayWhenZoomIn);
        _storedMainActionWhenZoomOut = new AutoAttackAction(_mainActionDelayWhenZoomOut);

        _subActionStrategy = new ManualAction(_subActionDelay);

        _mainResultStrategy = new SingleProjectileAttack(_camTransform, _range, _targetLayer, ownerAnimator, _animator, _muzzleFlash, false,
            _emptyCartridgeSpawner, true, _weaponName.ToString(), _muzzle, _penetratePower, _trajectoryLineEffect, _displacementSpreadMultiplyRatio, 
            _damageDictionary, OnGenerateNoiseRequest);

        _subResultStrategy = new ZoomStrategy(_scope, _cameraPositionWhenZoom, _zoomDuration, _delayUntilScopeActivated, _normalFieldOfView, _zoomFieldOfView, OnZoomEventCall);

        RecoilStorage storage = GameObject.FindWithTag("RecoilStorage").GetComponent<RecoilStorage>();
        RecoilMapData recoilData = storage.OnRecoilDataSendRequested<RecoilMapData>(_weaponName, EventCallPart.Both);

        _storedMainRecoilWhenZoomIn = new AutoRecoilGenerator(_mainActionDelayWhenZoomIn, _recoilRatioWhenZoomIn, recoilData);
        _storedMainRecoilWhenZoomOut = new AutoRecoilGenerator(_mainActionDelayWhenZoomIn, _recoilRatioWhenZoomOut, recoilData);


        _subRecoilStrategy = new NoRecoilGenerator();

        _reloadStrategy = new MagazineReload(_reloadFinishTime, _reloadExitTime, _weaponName.ToString(), _maxAmmoCountInMagazine, _animator, _ownerAnimator, OnReloadRequested);

        LinkEvent(player);
    }
}
