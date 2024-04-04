using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree.Nodes
{
    public class ChangeAngleOfSight : Node
    {
        public enum State
        {
            UpAngle,
            DownAngle
        }

        Transform _captureTransform;
        float _angle, _angleOffset, _angleChangeAmount;
        State state = State.UpAngle;

        public ChangeAngleOfSight(Transform captureTransform, float angleOffset, float angleChangeAmount)
        {
            _captureTransform = captureTransform;
            _angleOffset = angleOffset;
            _angleChangeAmount = angleChangeAmount;
            _angle = 0;
        }

        void resetAngle()
        {
            if (_angle >= _angleOffset) state = State.DownAngle;
            else if (_angle <= -_angleOffset) state = State.UpAngle;

            switch (state)
            {
                case State.UpAngle:
                    _angle += _angleChangeAmount;
                    break;
                case State.DownAngle:
                    _angle -= _angleChangeAmount;
                    break;
            }
        }

        public override NodeState Evaluate()
        {
            resetAngle();
            _captureTransform.localRotation = Quaternion.Euler(0, _angle, 0);
            return NodeState.SUCCESS;
        }

        public override void OnDisableRequested()
        {
            Debug.Log("OnDisableRequested");
            _captureTransform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}