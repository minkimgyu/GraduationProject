using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHoleEffect : LerpFadeEffect
{
    ParticleSystem _objectBurstEffect;
    SpriteRenderer _holeRenderer;

    [SerializeField]
    float waitForFade = 5f;

    float spaceBetweenWall = 0.001f;

    protected override void Awake()
    {
        base.Awake();
        _objectBurstEffect = GetComponentInChildren<ParticleSystem>();
        _holeRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public override void Initialize(Vector3 hitPosition, Vector3 hitNormal, Quaternion holeRotation)
    {
        transform.position = hitPosition + (hitNormal * spaceBetweenWall);
        transform.rotation = holeRotation * transform.rotation;
    }

    public override void PlayEffect()
    {
        _objectBurstEffect.Play();
        StartCoroutine(LerpFadeRoutine());
    }

    protected override IEnumerator LerpFadeRoutine()
    {
        yield return new WaitForSeconds(waitForFade);

        float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
        float increment = smoothness / duration; //The amount of change to apply.
        while (progress < 1)
        {
            _holeRenderer.color = Color.Lerp(startColor, endColor, progress);
            progress += increment;
            yield return new WaitForSeconds(smoothness);
        }

        DestroySelf();
    }
}
