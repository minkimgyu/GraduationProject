using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using FSM;
using System;

namespace AI.ZombieFSM
{
    public class MoveState : State
    {
        Func<Vector3, int, Vector3> ReturnNodePos;
        Action<Vector3, bool> FollowPath;

        Transform _myTransform;
        int _wanderOffset = 0;
        Vector3 _targetPos;

        public MoveState(Func<Vector3, int, Vector3> ReturnNodePos, Action<Vector3, bool> FollowPath, Transform myTransform, int wanderOffset)
        {
            this.ReturnNodePos = ReturnNodePos;
            this.FollowPath = FollowPath;

            _myTransform = myTransform;
            _wanderOffset = wanderOffset;
        }

        public override void OnStateEnter()
        {
            Debug.Log("Movetate");
            _targetPos = ReturnNodePos.Invoke(_myTransform.position, _wanderOffset);
        }

        public override void OnStateUpdate()
        {
            FollowPath?.Invoke(_targetPos, true);
        }
    }
}