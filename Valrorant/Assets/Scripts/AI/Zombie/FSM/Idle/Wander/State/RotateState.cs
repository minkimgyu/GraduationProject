using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using Random = UnityEngine.Random;

namespace AI.Wandering
{
    public class RotateState : State
    {
        Action<Vector3> View;
        Action Stop;
        Transform _myTrasform;
        Vector3 _circlePos;

        public RotateState(Action<Vector3> View, Action Stop, Transform myTrasform)
        {
            this.View = View;
            this.Stop = Stop;

            _myTrasform = myTrasform;
        }

        Vector3 ReturnCirclePositionAround(float angle)
        {
            Vector3 circlePos = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
            return circlePos + _myTrasform.position; // 각도에 따른 위치 계산
        }

        public override void OnStateEnter()
        {
            Debug.Log("RotateState");

            float angle = Random.Range(0f, 360f);
            _circlePos = ReturnCirclePositionAround(angle);
        }

        public override void OnStateUpdate()
        {
            Vector3 dir = (_circlePos - _myTrasform.position).normalized;
            View?.Invoke(dir);
            Stop?.Invoke();
        }
    }
}
