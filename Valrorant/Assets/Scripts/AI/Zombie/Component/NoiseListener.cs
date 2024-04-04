using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NoiseListener : BaseCaptureComponent<ITarget>
{
    Queue<Vector3> _noiseQueue = new Queue<Vector3>();
    Action OnNoiseReceived;
    int _maxQueueSize;

    public override void Initialize(float radius, int maxQueueSize, Action OnNoiseReceived)
    {
        _captureRadius = radius;
        _maxQueueSize = maxQueueSize;
        this.OnNoiseReceived = OnNoiseReceived;
    }

    protected override bool IsAlreadyContaining(ITarget target)
    {
        if (IsQueueEmpty() == true) return false;

        Transform targetTransform = target.ReturnTransform();
        return _noiseQueue.Contains(targetTransform.position);
    }

    public void ClearAllNoise()
    {
        _noiseQueue.Clear();
    }
    public bool IsQueueEmpty()
    {
        return _noiseQueue.Count == 0;
    }

    public Vector3 ReturnFrontNoise()
    {
        return _noiseQueue.Dequeue();
    }

    bool NowOverMaxQueueSize()
    {
        return _noiseQueue.Count > _maxQueueSize;
    }

    protected override void OnTargetEnter(ITarget target)
    {
        if(NowOverMaxQueueSize()) _noiseQueue.Dequeue(); // 최대 크기를 넘기면 Queue에서 빼줌

        Transform targetTransform = target.ReturnTransform();
        _noiseQueue.Enqueue(targetTransform.position);
        OnNoiseReceived?.Invoke();
    }
}
