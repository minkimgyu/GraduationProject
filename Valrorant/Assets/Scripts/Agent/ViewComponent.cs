using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agent.Component
{
    public class ViewComponent : RecoilReceiver
    {
        [SerializeField] private Transform _actorBone;
        [SerializeField] private Transform _direction;

        [SerializeField] private Transform _cameraHolder;
        private Transform _camPivot;
        private Transform _cam;

        //protected Vector2 _viewRotation;

        [SerializeField] private float _viewYRange = 40;
        [SerializeField] private Vector2 _viewSensitivity;

        [SerializeField] private Vector2 _cameraRotationMultiplier;
        [SerializeField] private Vector2 _actorBoneRotationMultiplier;

        private Vector2 CameraViewRotation { get { return _viewRotation + _cameraRotationMultiplier; } } // ī�޶� ȸ�� ������Ƽ
        private Vector2 ActorBoneViewRotation { get { return _viewRotation + _actorBoneRotationMultiplier; } } // �𵨸� ȸ�� ������Ƽ

        public void Initialize(float viewYRange, Vector2 viewSensitivity)
        {
            CameraContainer controller = FindObjectOfType<CameraContainer>();

            _camPivot = controller._cameraParent;
            _cam = controller._mainCamera.transform;
            _firePoint = controller._firePoint;

            _viewYRange = viewYRange;
            _viewSensitivity = viewSensitivity;
        }

        public override void OnRecoilRequested(Vector2 recoilForce)
        {
            base.OnRecoilRequested(recoilForce);

            _cameraRotationMultiplier = recoilForce * 0.5f; // ī�޶� ������ �ݸ� ����
            _actorBoneRotationMultiplier = recoilForce * 0.5f;
        }

        // �� �κ��� �÷��̾� ��Ʈ�ѷ����� ��������
        public void ResetView()
        {
            _viewRotation.x += Mathf.Lerp(_viewRotation.x, Input.GetAxisRaw("Mouse X"), _viewSensitivity.x * Time.deltaTime);
            _viewRotation.y = Mathf.Clamp(_viewRotation.y - (Input.GetAxisRaw("Mouse Y") * _viewSensitivity.y * Time.deltaTime), -_viewYRange, _viewYRange);

            _direction.rotation = Quaternion.Euler(0, ActorBoneViewRotation.x, 0);

            _actorBone.rotation = Quaternion.Euler(ActorBoneViewRotation.y, _direction.eulerAngles.y, 0);
        }

        public override void ResetCamera()
        {
            base.ResetCamera();
            _camPivot.position = _cameraHolder.position;
            _cam.rotation = Quaternion.Euler(CameraViewRotation.y, CameraViewRotation.x, 0);
        }
    }
}