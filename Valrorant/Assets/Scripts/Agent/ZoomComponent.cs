using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;

public class ZoomComponent : MonoBehaviour, IObserver<GameObject, bool, float, float, float, float, bool>
{
    [SerializeField] private Camera[] cameras;

    [SerializeField] private Transform cameraHolder;

    [SerializeField] private Transform cameraNormalTransform;
    [SerializeField] private Transform cameraZoomTransform;

    Coroutine zoomRoutine;

    public void Notify(GameObject scope, bool nowZoom, float zoomDuration, float scopeOnDelay, float normalFieldOfView, float zoomFieldOfView, bool isInstant)
    {
        StopZoomRoutine();

        if (isInstant)
        {
            scope.SetActive(nowZoom);

            if (nowZoom)
            {
                cameraHolder.position = cameraZoomTransform.position;
                for (int i = 0; i < cameras.Length; i++)
                {
                    cameras[i].fieldOfView = zoomFieldOfView;
                }
            }
            else
            {
                cameraHolder.position = cameraNormalTransform.position;
                for (int i = 0; i < cameras.Length; i++)
                {
                    cameras[i].fieldOfView = normalFieldOfView;
                }
            }
        }
        else
        {
            StopZoomRoutine();

            zoomRoutine = StartCoroutine(ZoomRoutine(scope, nowZoom, zoomDuration, scopeOnDelay, normalFieldOfView, zoomFieldOfView));
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
                for (int i = 0; i < cameras.Length; i++)
                {
                    cameras[i].fieldOfView = Mathf.Lerp(cameras[i].fieldOfView, zoomFieldOfView, progress);
                }

                cameraHolder.position = Vector3.Lerp(cameraHolder.position, cameraZoomTransform.position, progress);

                if (progress > scopeOnDelay && activeScope == false)
                {
                    scope.SetActive(nowZoom);
                    activeScope = true;
                }
            }
            else
            {
                for (int i = 0; i < cameras.Length; i++)
                {
                    cameras[i].fieldOfView = Mathf.Lerp(cameras[i].fieldOfView, normalFieldOfView, progress);
                }

                cameraHolder.position = Vector3.Lerp(cameraHolder.position, cameraNormalTransform.position, progress);

                if (activeScope == false)
                {
                    scope.SetActive(nowZoom);
                    activeScope = true;
                }
            }

            if (progress > 0.1 && activeScope == false)
            {
                scope.SetActive(nowZoom);
                activeScope = true;
            }

            progress += increment;
            yield return new WaitForSeconds(smoothness);
        }
    }

    void StopZoomRoutine()
    {
        if (zoomRoutine != null)
        {
            StopCoroutine(zoomRoutine);
            zoomRoutine = null;
        }
    }
}
