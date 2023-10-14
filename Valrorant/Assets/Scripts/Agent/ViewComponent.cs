using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewComponent : MonoBehaviour
{
    [SerializeField] private Transform actorBone;
    [SerializeField] private Transform direction;

    [SerializeField] private Transform cameraHolder;

    [SerializeField] private Transform cameraNormalTransform;
    [SerializeField] private Transform cameraZoomTransform;

    [SerializeField] private Transform camPivot;
    [SerializeField] private Transform cam;
    [SerializeField] private AimEvent aimEvent;

    public Transform Cam { get { return cam; } }

    [SerializeField]
    private float _viewXSensitivity = 60;

    [SerializeField]
    private float _viewYSensitivity = 60;

    [SerializeField]
    private float _viewClampYMax = 40;

    [SerializeField]
    private float _viewClampYMin = -40;

    private float rotationX;
    private float rotationY;

    Coroutine zoomRoutine;

    private void Start()
    {
        aimEvent.OnAimRequested = SetZoom;
        aimEvent.OnAimOffInstantlyRequested = SetZoom;
    }

    void StopZoomRoutine()
    {
        if (zoomRoutine != null)
        {
            StopCoroutine(zoomRoutine);
            zoomRoutine = null;
        }
    }

    public void SetZoom(GameObject scope, bool nowZoom)
    {
        StopZoomRoutine();

        scope.SetActive(nowZoom);

        if (nowZoom) cameraHolder.position = cameraZoomTransform.position;
        else cameraHolder.position = cameraNormalTransform.position;
    }

    public void SetZoom(GameObject scope, bool nowZoom, float zoomDuration, float scopeOnDelay)
    {
        StopZoomRoutine();

        zoomRoutine = StartCoroutine(ZoomRoutine(scope, nowZoom, zoomDuration, scopeOnDelay));
    }

    // 코루틴으로 에임 전환 구현
    IEnumerator ZoomRoutine(GameObject scope, bool nowZoom, float zoomDuration, float scopeOnDelay)
    {
        bool activeScope = false;
        float smoothness = 0.001f;

        float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
        float increment = smoothness / zoomDuration; //The amount of change to apply.
        while (progress < 1)
        {
            if(nowZoom)
            {
                cameraHolder.position = Vector3.Lerp(cameraHolder.position, cameraZoomTransform.position, progress);

                if (progress > scopeOnDelay && activeScope == false)
                {
                    scope.SetActive(nowZoom);
                    activeScope = true;
                }
            }
            else
            {
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

    public void ResetView()
    {
        rotationX = Mathf.Lerp(rotationX, Input.GetAxisRaw("Mouse X"), _viewXSensitivity * Time.deltaTime);
        rotationY = Mathf.Clamp(rotationY - (Input.GetAxisRaw("Mouse Y") * _viewYSensitivity * Time.deltaTime), _viewClampYMin, _viewClampYMax);

        direction.Rotate(0, rotationX, 0, Space.World);
        actorBone.rotation = Quaternion.Euler(rotationY, direction.eulerAngles.y, 0);
    }

    public void ResetCamera()
    {
        cam.rotation = Quaternion.Euler(rotationY, direction.eulerAngles.y, 0);
        camPivot.position = cameraHolder.position;
    }
}
