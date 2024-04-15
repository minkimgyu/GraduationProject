using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BehaviorTree.Nodes
{
    public class NowWithinActionRange : Node
    {
        enum State
        {
            WithinRange,
            OutsideOfRange
        }

        State _state;

        float _finalRange = 0;
        float _range;
        float _rangeOffset;
        Transform _myTransform;
        Func<ITarget> ReturnTargetInSight;

        public NowWithinActionRange(Transform myTransform, Func<ITarget> ReturnTargetInSight, float range, float rangeOffset)
        {
            this.ReturnTargetInSight = ReturnTargetInSight;

            _range = range;
            _rangeOffset = rangeOffset;
            _myTransform = myTransform;
        }

        void setState(State state)
        {
            if (_state == state) return;
            _state = state;

            switch (_state)
            {
                case State.WithinRange:
                    _finalRange = _range + _rangeOffset;
                    break;
                case State.OutsideOfRange:
                    _finalRange = _range;
                    break;
            }
        }

        public override NodeState Evaluate()
        {
            ITarget target = ReturnTargetInSight();
            if (target == null) return NodeState.FAILURE;

            float distanceFromTarget = Vector3.Distance(_myTransform.position, target.ReturnPos());
            if(distanceFromTarget < _finalRange)
            {
                setState(State.WithinRange);
                return NodeState.SUCCESS;
            }

            setState(State.OutsideOfRange);
            return NodeState.FAILURE;
        }
    }
}