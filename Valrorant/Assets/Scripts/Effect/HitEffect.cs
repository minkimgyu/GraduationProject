using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HitEffect : BaseEffect
{
    float spaceBetweenWall = 0.001f;

    [SerializeField]
    ParticleSystem _objectBurstEffect;

    public override void Initialize(Vector3 hitPosition)
    {
        transform.position = hitPosition;
    }

    public override void Initialize(Vector3 hitPosition, Vector3 hitNormal, Quaternion holeRotation)
    {
        transform.position = hitPosition + (hitNormal * spaceBetweenWall);
        transform.rotation = holeRotation * transform.rotation;
    }

    public override void PlayEffect()
    {
        _objectBurstEffect.Play();
        _timer.Start(_duration);
    }

    protected override void OnCollisionEnterRequested(Collision collision) { }

    protected override void OnUpdate()
    {
        _timer.Update();

        if (_timer.IsFinish)
        {
            DisableObject();
        }
    }
}
