using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoomComponent : MonoBehaviour//, IObserver<GameObject, bool, float, float, float, float, bool>
{
    [SerializeField] private Camera[] _cameras;
    private GameObject _crosshair;

    [SerializeField] private Transform _armMesh;

    private Vector3 _armLocalPosition;

    Coroutine _zoomRoutine;

    void Start()
    {
        _crosshair = GameObject.FindWithTag("Crosshair");
    }

    public void OnZoomCalled(GameObject scope, bool nowZoom, float zoomDuration, Vector3 armLocalPosition, float scopeOnDelay, float fieldOfView, bool isInstant)
    {
        StopZoomRoutine();
        _armLocalPosition = armLocalPosition;

        if (isInstant)
        {
            scope.SetActive(nowZoom);
            _crosshair.SetActive(!nowZoom);

            if (nowZoom) MoveCamera(fieldOfView, _armLocalPosition);
            else MoveCamera(fieldOfView, Vector3.zero);
        }
        else
        {
            _zoomRoutine = StartCoroutine(ZoomRoutine(scope, nowZoom, zoomDuration, scopeOnDelay, fieldOfView));
        }
    }

    // 코루틴으로 에임 전환 구현
    IEnumerator ZoomRoutine(GameObject scope, bool nowZoom, float zoomDuration, float scopeOnDelay, float fieldOfView)
    {
        bool isActivateFinish = false;
        float smoothness = 0.001f;

        float progress = 0;
        float increment = smoothness / zoomDuration;
        while (progress < 1)
        {
            if (nowZoom)
            {
                MoveCamera(fieldOfView, _armLocalPosition, progress);

                if (progress > scopeOnDelay && isActivateFinish == false)
                {
                    ActivateScope(scope, nowZoom);
                    isActivateFinish = true;
                }
            }
            else
            {
                MoveCamera(fieldOfView, Vector3.zero, progress);

                if (isActivateFinish == false)
                {
                    ActivateScope(scope, nowZoom);
                    isActivateFinish = true;
                }
            }

            progress += increment;
            yield return new WaitForSeconds(smoothness);
        }
    }

    void ActivateScope(GameObject scope, bool nowZoom)
    {
        _crosshair.SetActive(!nowZoom);
        scope.SetActive(nowZoom);
    }

    void MoveCamera(float fieldOfView, Vector3 armPosition, float progress)
    {
        for (int i = 0; i < _cameras.Length; i++)
        {
            _cameras[i].fieldOfView = Mathf.Lerp(_cameras[i].fieldOfView, fieldOfView, progress);
        }

        _armMesh.localPosition = Vector3.Lerp(_armMesh.localPosition, armPosition, progress);
    }

    void MoveCamera(float fieldOfView, Vector3 armPosition)
    {
        for (int i = 0; i < _cameras.Length; i++)
        {
            _cameras[i].fieldOfView = fieldOfView;
        }

        _armMesh.localPosition = armPosition;
    }

    void StopZoomRoutine()
    {
        if (_zoomRoutine != null)
        {
            StopCoroutine(_zoomRoutine);
            _zoomRoutine = null;
        }
    }
}
