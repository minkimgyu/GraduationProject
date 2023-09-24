using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryLineEffect : LerpFadeEffect
{
    LineRenderer _lineRenderer;

    protected override void Awake()
    {
        base.Awake();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public override void Initialize(Vector3 hitPosition, Vector3 shootPosition)
    {
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, shootPosition);
        _lineRenderer.SetPosition(1, hitPosition);
    }

    public override void PlayEffect()
    {
        StartCoroutine(LerpFadeRoutine());
    }

    protected override IEnumerator LerpFadeRoutine()
    {
        float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
        float increment = smoothness / duration; //The amount of change to apply.
        while (progress < 1)
        {
            _lineRenderer.startColor = Color.Lerp(startColor, endColor, progress * 2);
            _lineRenderer.endColor = Color.Lerp(startColor, endColor, progress);
            progress += increment;
            yield return new WaitForSeconds(smoothness);
        }

        DestroySelf();
    }
}
