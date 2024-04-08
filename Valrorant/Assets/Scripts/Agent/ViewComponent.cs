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

        [SerializeField] private Transform _camPivot;
        [SerializeField] private Transform _cam;

        protected Vector2 _viewRotation;

        [SerializeField] private float _viewClampYMax = 40;
        [SerializeField] private float _viewClampYMin = -40;
        [SerializeField] private Vector2 _viewSensitivity;

        [SerializeField] private Vector2 _cameraRotationMultiplier;
        [SerializeField] private Vector2 _actorBoneRotationMultiplier;

        private Vector2 CameraViewRotation { get { return _viewRotation + _cameraRotationMultiplier; } } // 카메라 회전 프로퍼티
        private Vector2 ActorBoneViewRotation { get { return _viewRotation + _actorBoneRotationMultiplier; } } // 모델링 회전 프로퍼티
        private Vector2 FireViewRotation { get { return _viewRotation + _firePointRotationMultiplier; } } // 총 발사 시작 지점 회전 값 프로퍼티

        public override void OnRecoilProgress(Vector2 recoilForce)
        {
            base.OnRecoilProgress(recoilForce);

            _cameraRotationMultiplier = recoilForce * 0.5f; // 카메라만 영향을 반만 받음

            _actorBoneRotationMultiplier = recoilForce * 0.5f;
        }

        public void ResetView()
        {
            _viewRotation.x += Mathf.Lerp(_viewRotation.x, Input.GetAxisRaw("Mouse X"), _viewSensitivity.x * Time.deltaTime);
            _viewRotation.y = Mathf.Clamp(_viewRotation.y - (Input.GetAxisRaw("Mouse Y") * _viewSensitivity.y * Time.deltaTime), _viewClampYMin, _viewClampYMax);

            _direction.rotation = Quaternion.Euler(0, ActorBoneViewRotation.x, 0);

            _actorBone.rotation = Quaternion.Euler(ActorBoneViewRotation.y, _direction.eulerAngles.y, 0);
        }

        public override void ResetCamera()
        {
            // _direction.eulerAngles.y
            _camPivot.position = _cameraHolder.position;
            _cam.rotation = Quaternion.Euler(CameraViewRotation.y, CameraViewRotation.x, 0);
            _firePoint.rotation = Quaternion.Euler(FireViewRotation.y, FireViewRotation.x, 0);
        }
    }
}