using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ZoomComponent : MonoBehaviour
{
    [SerializeField] private Transform _armMesh;

    Vector3 _zoomPos;
    float _fieldOfView;

    StopwatchTimer _timer;

    public void Initialize()
    {
        CameraController controller = FindObjectOfType<CameraController>();
        _timer = new StopwatchTimer();
    }

    public void OnZoomCalled(bool nowTurnOn, float zoomDuration, Vector3 zoomPos, float fieldOfView)
    {
        _zoomPos = zoomPos;
        _fieldOfView = fieldOfView;

        EventBusManager.Instance.ObserverEventBus.Publish
        (
            ObserverEventBus.Type.ActiveCrosshair,
            nowTurnOn
        );

        // 바로 Zoom으로 들어감
        if (zoomDuration == 0)
        {
            MoveCamera(_zoomPos, _fieldOfView);
            return;
        }

        if (_timer.CurrentState == StopwatchTimer.State.Running || _timer.CurrentState == StopwatchTimer.State.Finish) _timer.Reset();
        _timer.Start(zoomDuration);
    }

    /// <summary>
    /// 코루틴이 아닌 Update에서 타이머 돌리면서 적용시키기
    /// 
    /// OnZoomCalled에서 isInstant은 duration 조절해서 넣어줘도 될 듯
    /// 스코프는 달려있는 걸 사용할거라서 On, Off 기능 따로 안 넣어도 될 것 같다.
    /// </summary>
    public void OnUpdate()
    {
        if (_timer.CurrentState == StopwatchTimer.State.Finish) return;

        MoveCamera(_zoomPos, _fieldOfView, _timer.Ratio);
    }

    void MoveCamera(Vector3 armPosition, float fieldOfView, float progress = 1)
    {
        EventBusManager.Instance.ObserverEventBus.Publish
        (
            ObserverEventBus.Type.ChangeCameraFieldOfView,
            fieldOfView, progress
        );

        _armMesh.localPosition = Vector3.Lerp(_armMesh.localPosition, armPosition, progress);
    }
}
