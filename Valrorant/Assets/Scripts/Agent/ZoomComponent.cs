using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ObserverPattern;

public class ZoomComponent : MonoBehaviour//, IObserver<GameObject, bool, float, float, float, float, bool>
{
    [SerializeField] private Camera[] _cameras;
    private GameObject _crosshair;

    [SerializeField] private Transform _cameraHolder;

    [SerializeField] private Transform _cameraNormalTransform;
    [SerializeField] private Transform _cameraZoomTransform;

    Coroutine _zoomRoutine;

    void Start()
    {
        _crosshair = GameObject.FindWithTag("Crosshair");
    }

    public void OnZoomCalled(GameObject scope, bool nowZoom, float zoomDuration, Vector3 zoomCameraLocalPosition, float scopeOnDelay, float normalFieldOfView, float zoomFieldOfView, bool isInstant)
    {
        StopZoomRoutine();

        _cameraZoomTransform.localPosition = zoomCameraLocalPosition;

        if (isInstant)
        {
            scope.SetActive(nowZoom);
            _crosshair.SetActive(!nowZoom);

            if (nowZoom)
            {
                _cameraHolder.position = _cameraZoomTransform.position;
                for (int i = 0; i < _cameras.Length; i++)
                {
                    _cameras[i].fieldOfView = zoomFieldOfView;
                }
            }
            else
            {
                _cameraHolder.position = _cameraNormalTransform.position;
                for (int i = 0; i < _cameras.Length; i++)
                {
                    _cameras[i].fieldOfView = normalFieldOfView;
                }
            }
        }
        else
        {
            StopZoomRoutine();

            _zoomRoutine = StartCoroutine(ZoomRoutine(scope, nowZoom, zoomDuration, scopeOnDelay, normalFieldOfView, zoomFieldOfView));
        }
    }

    // 코루틴으로 에임 전환 구현
    IEnumerator ZoomRoutine(GameObject scope, bool nowZoom, float zoomDuration, float scopeOnDelay, float normalFieldOfView, float zoomFieldOfView)
    {
        bool activeScope = false;
        float smoothness = 0.001f;

        float progress = 0;
        float increment = smoothness / zoomDuration;
        while (progress < 1)
        {
            if (nowZoom)
            {
                for (int i = 0; i < _cameras.Length; i++)
                {
                    _cameras[i].fieldOfView = Mathf.Lerp(_cameras[i].fieldOfView, zoomFieldOfView, progress);
                }

                _cameraHolder.position = Vector3.Lerp(_cameraHolder.position, _cameraZoomTransform.position, progress);

                if (progress > scopeOnDelay && activeScope == false)
                {
                    _crosshair.SetActive(!nowZoom);

                    scope.SetActive(nowZoom);
                    activeScope = true;
                }
            }
            else
            {
                for (int i = 0; i < _cameras.Length; i++)
                {
                    _cameras[i].fieldOfView = Mathf.Lerp(_cameras[i].fieldOfView, normalFieldOfView, progress);
                }

                _cameraHolder.position = Vector3.Lerp(_cameraHolder.position, _cameraNormalTransform.position, progress);

                if (activeScope == false)
                {
                    _crosshair.SetActive(!nowZoom);

                    scope.SetActive(nowZoom);
                    activeScope = true;
                }
            }

            progress += increment;
            yield return new WaitForSeconds(smoothness);
        }
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
