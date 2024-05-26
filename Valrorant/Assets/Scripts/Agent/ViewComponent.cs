using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Agent.Component
{
    public class ViewComponent : MonoBehaviour, IRecoilReceiver
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

        [SerializeField] protected Transform _firePoint;

        [SerializeField] protected Vector2 _firePointRotationMultiplier;
        Vector2 _viewRotation;

        public Vector2 FireViewRotation { get { return _viewRotation + _firePointRotationMultiplier; } } // 총 발사 시작 지점 회전 값 프로퍼티

        private Vector2 CameraViewRotation { get { return _viewRotation + _cameraRotationMultiplier; } } // 카메라 회전 프로퍼티
        private Vector2 ActorBoneViewRotation { get { return _viewRotation + _actorBoneRotationMultiplier; } } // 모델링 회전 프로퍼티

        public void Initialize(float viewYRange, Vector2 viewSensitivity)
        {
            CameraContainer controller = FindObjectOfType<CameraContainer>();

            _camPivot = controller._cameraParent;
            _cam = controller._mainCamera.transform;
            _firePoint = controller._firePoint;

            _viewYRange = viewYRange;
            _viewSensitivity = viewSensitivity;
        }

        public void OnRecoilRequested(Vector2 recoilForce)
        {
            _firePointRotationMultiplier = recoilForce * 0.5f;
            _cameraRotationMultiplier = recoilForce * 0.5f; // 카메라만 영향을 반만 받음
            _actorBoneRotationMultiplier = recoilForce * 0.5f;
        }

        // 이 부분은 플레이어 컨트롤러에서 돌려주자
        public void ResetView()
        {
            _viewRotation.x += Mathf.Lerp(_viewRotation.x, Input.GetAxisRaw("Mouse X"), _viewSensitivity.x * Time.deltaTime);
            _viewRotation.y = Mathf.Clamp(_viewRotation.y - (Input.GetAxisRaw("Mouse Y") * _viewSensitivity.y * Time.deltaTime), -_viewYRange, _viewYRange);

            _direction.rotation = Quaternion.Euler(0, ActorBoneViewRotation.x, 0);
            _actorBone.rotation = Quaternion.Euler(ActorBoneViewRotation.y, _direction.eulerAngles.y, 0);
        }

        public void ApplyRecoilToCamera()
        {
            _firePoint.rotation = Quaternion.Euler(FireViewRotation.y, FireViewRotation.x, 0);
            _camPivot.position = _cameraHolder.position;
            _cam.rotation = Quaternion.Euler(CameraViewRotation.y, CameraViewRotation.x, 0);
        }

        public Vector3 ReturnRaycastPos() { return _firePoint.position; }

        public Vector3 ReturnRaycastDir() { return _firePoint.forward; }
    }
}