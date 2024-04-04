using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree.Nodes
{
    public class WaitForStateChange : Node
    {
        StopwatchTimer _stopwatchTimer;
        float _delayDuration = 0.5f;

        public WaitForStateChange(float delayDuration)
        {
            _stopwatchTimer = new StopwatchTimer();
            _delayDuration = delayDuration;
            _stopwatchTimer.Start(_delayDuration);
        }

        public override NodeState Evaluate()
        {
            switch (_stopwatchTimer.CurrentState)
            {
                case StopwatchTimer.State.Ready:

                    _stopwatchTimer.Start(_delayDuration);
                    return NodeState.SUCCESS;
                case StopwatchTimer.State.Running:

                    return NodeState.RUNNING;
                case StopwatchTimer.State.Finish:

                    _stopwatchTimer.Reset();
                    _stopwatchTimer.Start(_delayDuration);
                    return NodeState.SUCCESS;
            }

            return NodeState.FAILURE;
        }

        public override void OnDisableRequested()
        {
            _stopwatchTimer.Reset();
        }
    }
}