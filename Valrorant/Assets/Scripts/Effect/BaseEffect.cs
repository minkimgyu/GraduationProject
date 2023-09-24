using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseEffect : MonoBehaviour
{
    public virtual void Initialize(Vector3 hitPosition, Vector3 shootPosition) { }

    public virtual void Initialize(Vector3 hitPosition, Vector3 hitNormal, Quaternion holeRotation) { }

    abstract public void PlayEffect();

    protected void DestroySelf() => Destroy(gameObject);
}

abstract public class LerpFadeEffect : BaseEffect
{
    [SerializeField]
    protected Color startColor;

    [SerializeField]
    protected Color endColor;

    [SerializeField]
    protected float duration = 0.1f;

    protected float smoothness = 0.001f;

    WaitForSeconds waitTime;

    protected virtual void Awake()
    {
        waitTime = new WaitForSeconds(smoothness);
    }

    abstract protected IEnumerator LerpFadeRoutine();
}