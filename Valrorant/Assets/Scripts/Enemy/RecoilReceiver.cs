using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilReceiver : MonoBehaviour
{
    [SerializeField] protected Transform _firePoint;

    [SerializeField] protected Vector2 _firePointRotationMultiplier;

    public virtual void OnRecoilProgress(Vector2 recoilForce)
    {
        _firePointRotationMultiplier = recoilForce;
    }

    public virtual void ResetCamera() { }
}
