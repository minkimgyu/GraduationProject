using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviorTree.Nodes
{
    public class EvaluatingDistance : Node
    {
        protected enum State
        {
            WithinRange,
            OutOfRange,
        }


        protected Transform _myTransform;
        protected float _distance;
        protected float _distanceOffset;

        bool _checkFarAway;

        protected Action OnOutOfRangeRequested;

        protected State _state;

        public EvaluatingDistance(Transform myTransform, float farDistance, float farDistanceOffset, bool checkFarAway)
        {
            _state = State.WithinRange;

            _myTransform = myTransform;
            _distance = farDistance;
            _distanceOffset = farDistanceOffset;
            _checkFarAway = checkFarAway;
        }

        public EvaluatingDistance(Transform myTransform, float farDistance, float farDistanceOffset, bool checkFarAway, Action OnOutOfRangeRequested)
        {
            _state = State.WithinRange;

            _myTransform = myTransform;
            _distance = farDistance;
            _distanceOffset = farDistanceOffset;
            _checkFarAway = checkFarAway;

            this.OnOutOfRangeRequested = OnOutOfRangeRequested;
        }

        // 상황에 따라서 손 봐야할 듯
        protected void SwitchState(Vector3 targetPos)
        {
            float distance = Vector3.Distance(_myTransform.position, targetPos);

            switch (_state)
            {
                case State.WithinRange:
                    if (distance <= _distance) break;

                    if(_checkFarAway) _distance -= _distanceOffset;
                    else _distance += _distanceOffset;

                    _state = State.OutOfRange;
                    OnOutOfRangeRequested?.Invoke();

                    break;
                case State.OutOfRange:
                    if (distance > _distance) break;

                    if (_checkFarAway) _distance += _distanceOffset;
                    else _distance -= _distanceOffset;

                    _state = State.WithinRange;
                    break;
            }
        }
    }
}