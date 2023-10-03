using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHoleEffect : CoroutineEffect
{
    [SerializeField]
    protected Color startColor;

    [SerializeField]
    protected Color endColor;

    [SerializeField]
    float waitForFade = 5f;

    ParticleSystem _objectBurstEffect;
    SpriteRenderer _holeRenderer;

    float spaceBetweenWall = 0.001f;

    protected override void Awake()
    {
        base.Awake();
        _objectBurstEffect = GetComponentInChildren<ParticleSystem>();
        _holeRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public override void Initialize(Vector3 hitPosition, Vector3 hitNormal, Quaternion holeRotation)
    {
        _holeRenderer.color = startColor;
        transform.position = hitPosition + (hitNormal * spaceBetweenWall);
        transform.rotation = holeRotation * transform.rotation;
    }

    public override void PlayEffect()
    {
        _objectBurstEffect.Play();
        StartCoroutine(Routine());
    }

    protected override IEnumerator Routine()
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

        DisableObject();
    }
}
