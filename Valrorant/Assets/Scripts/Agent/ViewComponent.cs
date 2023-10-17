using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;

public class ViewComponent : MonoBehaviour, IObserver<Vector2, Vector2, Vector2>
{
    [SerializeField] private Transform actorBone;
    [SerializeField] private Transform direction;

    [SerializeField] private Transform cameraHolder;

    [SerializeField] private Transform camPivot;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform firePoint;

    public Transform FirePoint { get { return firePoint; } }

    [SerializeField]
    private float _viewClampYMax = 40;

    [SerializeField]
    private float _viewClampYMin = -40;

    [SerializeField]
    private Vector2 _viewSensitivity;

    private Vector2 _viewRotation;

    [SerializeField]
    private Vector2 _cameraRotationMultiplier;

    [SerializeField]
    private Vector2 _firePointRotationMultiplier;

    [SerializeField]
    private Vector2 _actorBoneRotationMultiplier;

    private Vector2 CameraViewRotation { get { return _viewRotation + _cameraRotationMultiplier; } } // 카메라용
    private Vector2 ActorBoneViewRotation { get { return _viewRotation + _actorBoneRotationMultiplier; } } // 카메라용
    private Vector2 FireViewRotation { get { return _viewRotation + _firePointRotationMultiplier; } } // 카메라용

    public void Notify(Vector2 cameraForce, Vector2 recoilForce, Vector2 actorBoneForce)
    {
        _cameraRotationMultiplier += cameraForce; // 합연산으로 처리 --> 복수의 반동도 제어 가능
        _firePointRotationMultiplier += recoilForce;
        _actorBoneRotationMultiplier += actorBoneForce;
    }

    public void ResetView()
    {
        _viewRotation.x = Mathf.Lerp(_viewRotation.x, Input.GetAxisRaw("Mouse X"), _viewSensitivity.x * Time.smoothDeltaTime);
        _viewRotation.y = Mathf.Clamp(_viewRotation.y - (Input.GetAxisRaw("Mouse Y") * _viewSensitivity.y * Time.smoothDeltaTime), _viewClampYMin, _viewClampYMax);

        // Input.GetAxisRaw("Mouse Y") * _viewSensitivity.y * Time.smoothDeltaTime를 누적해서 값을 저장하는데 이 값이 min, max 값을 넘으면 안 됨
        // 그래서 Clamp를 넣어서 이 이상 못 넘게 막아줌
        // 게임 시작 시, 움직이면 불규칙하게 값이 튐 --> 수정 가능성

        direction.rotation = Quaternion.Euler(0, direction.rotation.eulerAngles.y + CameraViewRotation.x, 0);
        actorBone.rotation = Quaternion.Euler(ActorBoneViewRotation.y, direction.eulerAngles.y, 0);
    }

    public void ResetCamera()
    {
        camPivot.position = cameraHolder.position;
        cam.rotation = Quaternion.Euler(CameraViewRotation.y, direction.eulerAngles.y, 0);
        firePoint.rotation = Quaternion.Euler(FireViewRotation.y, direction.eulerAngles.y, 0);
    }
}
