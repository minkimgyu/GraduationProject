using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ZoomComponent : MonoBehaviour//, IObserver<GameObject, bool, float, float, float, float, bool>
{
    [SerializeField] private Camera[] _cameras;
    [SerializeField] private Transform _armMesh;

    Vector3 _zoomPos;
    float _fieldOfView;

    StopwatchTimer _timer;

    Action<bool> SwitchCrosshair;

    public void Initialize()
    {
        CameraContainer controller = FindObjectOfType<CameraContainer>();
        _cameras[0] = controller._mainCamera;
        _cameras[1] = controller._subCamera;

        _timer = new StopwatchTimer();

        CrosshairController crosshairController = FindObjectOfType<CrosshairController>();
        if (crosshairController == null) return;
        SwitchCrosshair = crosshairController.SwitchCrosshair;
    }

    public void OnZoomCalled(bool nowTurnOn, float zoomDuration, Vector3 zoomPos, float fieldOfView)
    {
        _zoomPos = zoomPos;
        _fieldOfView = fieldOfView;

        SwitchCrosshair?.Invoke(nowTurnOn);

        // �ٷ� Zoom���� ��
        if (zoomDuration == 0)
        {
            MoveCamera(_zoomPos, _fieldOfView);
            return;
        }

        if (_timer.CurrentState == StopwatchTimer.State.Running || _timer.CurrentState == StopwatchTimer.State.Finish) _timer.Reset();
        _timer.Start(zoomDuration);
    }

    /// <summary>
    /// �ڷ�ƾ�� �ƴ� Update���� Ÿ�̸� �����鼭 �����Ű��
    /// 
    /// OnZoomCalled���� isInstant�� duration �����ؼ� �־��൵ �� ��
    /// �������� �޷��ִ� �� ����ҰŶ� On, Off ��� ���� �� �־ �� �� ����.
    /// </summary>
    public void OnUpdate()
    {
        if (_timer.CurrentState == StopwatchTimer.State.Finish) return;

        MoveCamera(_zoomPos, _fieldOfView, _timer.Ratio);
    }

    void MoveCamera(Vector3 armPosition, float fieldOfView, float progress = 1)
    {
        for (int i = 0; i < _cameras.Length; i++)
        {
            _cameras[i].fieldOfView = Mathf.Lerp(_cameras[i].fieldOfView, fieldOfView, progress);
        }

        _armMesh.localPosition = Vector3.Lerp(_armMesh.localPosition, armPosition, progress);
    }

    //public void OnZoomCalled(GameObject scope, bool nowZoom, float zoomDuration, Vector3 armLocalPosition,
    //float scopeOnDelay, float fieldOfView, bool isInstant)
    //{
    //    StopZoomRoutine();
    //    _armLocalPosition = armLocalPosition;

    //    if (isInstant)
    //    {
    //        scope.SetActive(nowZoom);
    //        _crosshair.SetActive(!nowZoom);

    //        if (nowZoom) MoveCamera(fieldOfView, _armLocalPosition);
    //        else MoveCamera(fieldOfView, Vector3.zero);
    //    }
    //    else
    //    {
    //        _zoomRoutine = StartCoroutine(ZoomRoutine(scope, nowZoom, zoomDuration, scopeOnDelay, fieldOfView));
    //    }
    //}

    //// �ڷ�ƾ���� ���� ��ȯ ����
    //IEnumerator ZoomRoutine(GameObject scope, bool nowZoom, float zoomDuration, float scopeOnDelay, float fieldOfView)
    //{
    //    bool isActivateFinish = false;
    //    float smoothness = 0.001f;

    //    float progress = 0;
    //    float increment = smoothness / zoomDuration;
    //    while (progress < 1)
    //    {
    //        if (nowZoom)
    //        {
    //            MoveCamera(fieldOfView, _armLocalPosition, progress);

    //            if (progress > scopeOnDelay && isActivateFinish == false)
    //            {
    //                ActivateScope(scope, nowZoom);
    //                isActivateFinish = true;
    //            }
    //        }
    //        else
    //        {
    //            MoveCamera(fieldOfView, Vector3.zero, progress);

    //            if (isActivateFinish == false)
    //            {
    //                ActivateScope(scope, nowZoom);
    //                isActivateFinish = true;
    //            }
    //        }

    //        progress += increment;
    //        yield return new WaitForSeconds(smoothness);
    //    }
    //}

    //void ActivateScope(GameObject scope, bool nowZoom)
    //{
    //    _crosshair.SetActive(!nowZoom);
    //    scope.SetActive(nowZoom);
    //}

    //void StopZoomRoutine()
    //{
    //    if (_zoomRoutine != null)
    //    {
    //        StopCoroutine(_zoomRoutine);
    //        _zoomRoutine = null;
    //    }
    //}
}
