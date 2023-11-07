using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FadingOutHitEffect : HitEffect
{
    [SerializeField]
    protected Color startColor;

    [SerializeField]
    protected Color endColor;

    [SerializeField]
    SpriteRenderer _holeRenderer;

    public override void Initialize(Vector3 hitPosition, Vector3 hitNormal, Quaternion holeRotation)
    {
        base.Initialize(hitPosition, hitNormal, holeRotation);
        _holeRenderer.color = startColor;
    }

    void ChangeColor(float progress)
    {
        _holeRenderer.color = Color.Lerp(startColor, endColor, progress);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        ChangeColor(_timer.Ratio);
    }
}
