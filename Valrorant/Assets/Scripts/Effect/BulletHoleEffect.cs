using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHoleEffect : HitEffect
{
    [SerializeField]
    protected Color startColor;

    [SerializeField]
    protected Color endColor;

    SpriteRenderer _holeRenderer;

    protected override void Awake()
    {
        base.Awake();
        _holeRenderer = GetComponentInChildren<SpriteRenderer>();
        runningRoutine.RoutineAction += ChangeColor;
    }

    public override void Initialize(Vector3 hitPosition, Vector3 hitNormal, Quaternion holeRotation)
    {
        base.Initialize(hitPosition, hitNormal, holeRotation);
        _holeRenderer.color = startColor;
    }

    void ChangeColor(float progress)
    {
        _holeRenderer.color = Color.Lerp(startColor, endColor, progress);
    }
}
