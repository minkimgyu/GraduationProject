using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;
using ObserverPattern;

[System.Serializable]
public class Phantom : Gun, IObserver<GameObject, bool, float, float, float, float, bool>
{
    [SerializeField]
    int mainAttackBulletCount = 1;

    [SerializeField]
    float _mainActionDelay;

    [SerializeField]
    float _subActionkDelay;

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

    RecoilStrategy _storedAutoRecoilGenerator;

    RecoilStrategy _storedZoomRecoilGenerator;


    Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> _damageDictionary = new Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]>()
    {
        { DistanceAreaData.HitArea.Head, new DistanceAreaData[]{ new DistanceAreaData(0, 15, 156), new DistanceAreaData(15, 30, 140), new DistanceAreaData(30, 50, 124) } },
        { DistanceAreaData.HitArea.Body, new DistanceAreaData[]{ new DistanceAreaData(0, 15, 39), new DistanceAreaData(15, 30, 35), new DistanceAreaData(30, 50, 31) } },
        { DistanceAreaData.HitArea.Leg, new DistanceAreaData[]{ new DistanceAreaData(0, 15, 33), new DistanceAreaData(15, 30, 29), new DistanceAreaData(30, 50, 26) } },
    };


    public override void Initialize(GameObject player, Transform cam, Animator ownerAnimator)
    {
        base.Initialize(player, cam, ownerAnimator);

        // ���⿡ Action �����ؼ� �Ѿ��� �Ҹ�Ǵ� �κ��� �����غ���
        _mainAction = new AutoAttackAction(_mainActionDelay); 
        _subAction = new ManualAction(_subActionkDelay);

        _mainResult = new SingleProjectileAttack(_camTransform, _range, _hitEffectName,
            _targetLayer, _muzzle, _penetratePower, _nonPenetrateHitEffect, _trajectoryLineEffect, _damageDictionary);

        Zoom zoom = new Zoom(_scope, _zoomDuration, _scopeOnDelay, _normalFieldOfView, _zoomFieldOfView);

        IObserver<GameObject, bool, float, float, float, float, bool> scopeObserver = player.GetComponent<IObserver<GameObject, bool, float, float, float, float, bool>>();
        zoom.AddObserver(scopeObserver);
        zoom.AddObserver(this);
        _subResult = zoom;


        IObserver<Vector2, Vector2, Vector2> _recoilObserver = player.GetComponent<IObserver<Vector2, Vector2, Vector2>>();


        AutoRecoilGenerator autoRecoilGenerator = new AutoRecoilGenerator(_recoilMap.RecoilRecoverDuration, _mainActionDelay, _recoilMap);
        autoRecoilGenerator.AddObserver(_recoilObserver);

        _storedAutoRecoilGenerator = autoRecoilGenerator;


        ZoomRecoilGenerator zoomRecoilGenerator = new ZoomRecoilGenerator(_recoilMap.RecoilRecoverDuration, _mainActionDelay, _recoilMap);
        zoomRecoilGenerator.AddObserver(_recoilObserver);

        _storedZoomRecoilGenerator = zoomRecoilGenerator;


        _mainRecoilGenerator = _storedAutoRecoilGenerator;
        _subRecoilGenerator = new NoRecoilGenerator();

        //_mainAction.OnStart ---> ���⿡�� �ʱ� View ���� ��Ƽ� ���߿� Lerp ���������
        _scope.SetActive(false);
        LinkActionStrategy();
    }

    public override void OnUnEquip()
    {
        base.OnUnEquip();
        _subResult.Do(false, true);
    }

    public override void OnReload()
    {
        base.OnReload();
        _subResult.Do(false, true);
    }

    // ����
    protected override void ChainMainActionProgressEvent()
    {
        Fire(_mainResult, _mainRecoilGenerator);
        PlayMainActionAnimation();
    }

    protected override void ChainSubActionStartEvent()
    {
        _subResult.Do();
        PlaySubActionAnimation();
    }

    public void Notify(GameObject scope, bool nowZoom, float zoomDuration, float scopeOnDelay, float normalFieldOfView, float zoomFieldOfView, bool isInstant)
    {
        if (nowZoom)
        {
            _mainRecoilGenerator = _storedZoomRecoilGenerator;
        }
        else
        {
            _mainRecoilGenerator = _storedAutoRecoilGenerator;
        }
    }
}
