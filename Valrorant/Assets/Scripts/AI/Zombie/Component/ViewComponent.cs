using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Component
{
    public class ViewComponent : RecoilReceiver
    {
        float _speed;
        [SerializeField] Transform _sightPoint;

        public void Initialize(float speed)
        {
            _speed = speed;
        }

        public void View(Vector3 dir)
        {
            transform.forward = Vector3.Lerp(transform.forward, dir, Time.smoothDeltaTime * _speed);
        }

        public override void ResetCamera()
        {
            _viewRotation = new Vector2(_sightPoint.rotation.eulerAngles.y, _sightPoint.rotation.eulerAngles.x);
            base.ResetCamera();
        }
    }
}