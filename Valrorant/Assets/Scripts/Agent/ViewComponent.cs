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

    private Vector2 CameraViewRotation { get { return _viewRotation + _cameraRotationMultiplier; } } // ī�޶��
    private Vector2 ActorBoneViewRotation { get { return _viewRotation + _actorBoneRotationMultiplier; } } // ī�޶��
    private Vector2 FireViewRotation { get { return _viewRotation + _firePointRotationMultiplier; } } // ī�޶��

    public void Notify(Vector2 cameraForce, Vector2 recoilForce, Vector2 actorBoneForce)
    {
        _cameraRotationMultiplier += cameraForce; // �տ������� ó�� --> ������ �ݵ��� ���� ����
        _firePointRotationMultiplier += recoilForce;
        _actorBoneRotationMultiplier += actorBoneForce;
    }

    public void ResetView()
    {
        _viewRotation.x = Mathf.Lerp(_viewRotation.x, Input.GetAxisRaw("Mouse X"), _viewSensitivity.x * Time.smoothDeltaTime);
        _viewRotation.y = Mathf.Clamp(_viewRotation.y - (Input.GetAxisRaw("Mouse Y") * _viewSensitivity.y * Time.smoothDeltaTime), _viewClampYMin, _viewClampYMax);

        // Input.GetAxisRaw("Mouse Y") * _viewSensitivity.y * Time.smoothDeltaTime�� �����ؼ� ���� �����ϴµ� �� ���� min, max ���� ������ �� ��
        // �׷��� Clamp�� �־ �� �̻� �� �Ѱ� ������
        // ���� ���� ��, �����̸� �ұ�Ģ�ϰ� ���� Ʀ --> ���� ���ɼ�

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
