using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace AI.Component
{
    public class MoveComponent : MonoBehaviour
    {
        Rigidbody _rigid;
        float _speed;

        Action<string, float> ResetAnimatorValue;
        Func<string, float> GetAnimatorValue;
        Transform _sightPoint;

        public void Initialize(Transform sightPoint, float speed, Action<string, float> ResetAnimatorValue, Func<string, float> GetAnimatorValue)
        {
            _sightPoint = sightPoint;
            _speed = speed;
            this.ResetAnimatorValue = ResetAnimatorValue;
            this.GetAnimatorValue = GetAnimatorValue;
            _rigid = GetComponent<Rigidbody>();
        }

        public float SendMoveDisplacement() { return _rigid.velocity.magnitude * 0.1f; }

        public Vector3 ReturnVelocity()
        {
            return _rigid.velocity;
        }

        public void Move(Vector3 dir)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + dir * _speed, Time.deltaTime);
            //_rigid.velocity = Vector3.Lerp(_rigid.velocity, dir * _speed, Time.de);

            //Vector3 changeDir = transform.TransformDirection(dir);
            //Debug.DrawRay(transform.position, changeDir);


            Vector3 changeDir = transform.InverseTransformDirection(dir);
            Debug.DrawRay(transform.position, changeDir);

            ResetAnimatorValue?.Invoke("Y", changeDir.z);
            ResetAnimatorValue?.Invoke("X", changeDir.x);
        }

        public void Stop()
        {
            float xValue = GetAnimatorValue("X");
            float yValue = GetAnimatorValue("Y");

            ResetAnimatorValue?.Invoke("X", Mathf.Lerp(xValue, 0, Time.deltaTime * 5));
            ResetAnimatorValue?.Invoke("Y", Mathf.Lerp(yValue, 0, Time.deltaTime * 5));
        }
    }
}