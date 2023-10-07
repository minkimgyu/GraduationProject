using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewComponent : MonoBehaviour
{
    [SerializeField] private Transform actorBone;
    [SerializeField] private Transform direction;

    private Transform cameraHolder;

    [SerializeField] private Transform cameraNormalTransform;
    [SerializeField] private Transform cameraZoomTransform;

    [SerializeField] private Transform camPivot;
    [SerializeField] private Transform cam;

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

    public void SetZoom(bool nowZoom)
    {
        if (nowZoom) cameraHolder = cameraZoomTransform;
        else cameraHolder = cameraNormalTransform;
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
