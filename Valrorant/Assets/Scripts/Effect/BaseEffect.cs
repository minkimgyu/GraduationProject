using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseEffect : MonoBehaviour
{
    public virtual void Initialize(Vector3 hitPosition, Vector3 shootPosition) { }

    public virtual void Initialize(Vector3 hitPosition, Vector3 hitNormal, Quaternion holeRotation) { }

    public virtual void Initialize(Vector3 hitPosition, Vector3 hitNormal, Quaternion holeRotation, float damamge) { }

    abstract public void PlayEffect();

    private void OnDisable()
    {
        ObjectPooler.ReturnToPool(gameObject);
    }

    protected void DisableObject() => gameObject.SetActive(false);
}

//abstract public class CoroutineEffect : BaseEffect
//{
//    [SerializeField]
//    protected float duration;

//    protected float smoothness = 0.001f;

//    WaitForSeconds waitTime;

//    protected virtual void Awake()
//    {
//        waitTime = new WaitForSeconds(smoothness);
//    }

//    abstract protected IEnumerator Routine();
//}