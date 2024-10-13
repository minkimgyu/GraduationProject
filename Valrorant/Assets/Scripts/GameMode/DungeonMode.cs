using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMode : BaseGameMode
{
    [SerializeField] CrosshairController _crosshairController;
    [SerializeField] CameraController _cameraController;
    [SerializeField] HpViewer _hpViewer;
    [SerializeField] RoundViwer _roundViwer;
    [SerializeField] WeaponViewer _weaponViewer;
    [SerializeField] HelperViewer _helperViewer;

    [SerializeField] WeaponInfoViwer _weaponInfoViwer;

    // 체력 변화
    // 크로스해어 온오프
    // 카메라 컨트롤
    // 총알 개수 조절
    // 
    protected override void Initialize()
    {
        InitializeEventBus();
    }

    private void InitializeEventBus()
    {
        ObserverEventBus observerEventBus = new ObserverEventBus();
        EventBusManager.Instance.Initialize(observerEventBus);

        EventBusManager.Instance.ObserverEventBus.Register
        (
            ObserverEventBus.Type.ChangeHp,
            new ChangeRatioCommand(_hpViewer.OnHpChange)
        );



        EventBusManager.Instance.ObserverEventBus.Register
        (
            ObserverEventBus.Type.ChangeAmmo,
            new ChangeAmmoCommand(_roundViwer.OnRoundCountChange)
        );



        EventBusManager.Instance.ObserverEventBus.Register
        (
            ObserverEventBus.Type.ActiveCrosshair,
            new ActiveCommand(_crosshairController.SwitchCrosshair)
        );

        EventBusManager.Instance.ObserverEventBus.Register
        (
            ObserverEventBus.Type.MoveCamera,
            new MoveCameraCommand(_cameraController.MoveCamera)
        );

        EventBusManager.Instance.ObserverEventBus.Register
        (
            ObserverEventBus.Type.ChangeCameraFieldOfView,
            new ChangeFieldOfViewCommand(_cameraController.ChangeFieldOfView)
        );
    }

    public override void OnGameClearRequested()
    {
    }

    public override void OnGameOverRequested()
    {
       
    }
}