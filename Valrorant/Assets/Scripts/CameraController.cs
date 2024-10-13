using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform _cameraParent;
    [SerializeField] Camera[] _cameras;
    [SerializeField] Transform _firePoint;

    public void MoveCamera(Vector3 cameraHolderPosition, Vector3 viewRotation)
    {
        transform.position = cameraHolderPosition;
        transform.rotation = Quaternion.Euler(viewRotation.y, viewRotation.x, 0);
    }

    public void ChangeFieldOfView(float fieldOfView, float progress)
    {
        for (int i = 0; i < _cameras.Length; i++)
        {
            _cameras[i].fieldOfView = Mathf.Lerp(_cameras[i].fieldOfView, fieldOfView, progress);
        }
    }
}
