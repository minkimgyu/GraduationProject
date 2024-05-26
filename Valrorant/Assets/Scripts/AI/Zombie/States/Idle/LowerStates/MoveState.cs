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
        Action<Vector3, List<Vector3>, bool> FollowPath;

        Transform _myTransform;
        int _wanderOffset = 0;
        Vector3 _targetPos;

        public MoveState(Action<Vector3, List<Vector3>, bool> FollowPath, Transform myTransform, int wanderOffset)
        {
            this.FollowPath = FollowPath;

            _myTransform = myTransform;
            _wanderOffset = wanderOffset;
        }

        public override void OnStateEnter()
        {
            //Debug.Log("Movetate");
            int xOffset = Random.Range(-_wanderOffset, _wanderOffset);
            int zOffset = Random.Range(-_wanderOffset, _wanderOffset);

            _targetPos = _myTransform.position + new Vector3(zOffset, 0, xOffset);
        }

        public override void OnStateUpdate()
        {
            FollowPath?.Invoke(_targetPos, null, true);
        }
    }
}