using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using FSM;
using System;

namespace AI.Wandering
{
    public class MoveState : State
    {
        Func<Vector3, Vector3> ReturnNodePos;
        Action<Vector3, bool> FollowPath;

        Transform _myTransform;
        int _wanderOffset = 0;
        Vector3 _targetPos;

        public MoveState(Func<Vector3, Vector3> ReturnNodePos, Action<Vector3, bool> FollowPath, Transform myTransform, int wanderOffset)
        {
            this.ReturnNodePos = ReturnNodePos;
            this.FollowPath = FollowPath;

            _myTransform = myTransform;
            _wanderOffset = wanderOffset;
        }

        public override void OnStateEnter()
        {
            //Debug.Log("MoveState");
            Vector3 myPos = _myTransform.position;

            int xOffset = Random.Range(-_wanderOffset, _wanderOffset);
            int zOffset = Random.Range(-_wanderOffset, _wanderOffset);

            _targetPos = ReturnNodePos.Invoke(myPos + new Vector3(xOffset, 0, zOffset));
        }

        public override void OnStateUpdate()
        {
            FollowPath(_targetPos, false);
        }
    }
}