using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise : MonoBehaviour, ITarget
{
    BaseDrawer _sphereDrawer;
    SphereCollider _sphereCollider;

    public TargetType MyType { get; set; }

    public void Initialize(float radius, float disableTime)
    {
        MyType = TargetType.Sound;

        _sphereDrawer = GetComponent<BaseDrawer>();
        _sphereCollider = GetComponent<SphereCollider>();

        _sphereDrawer.ResetData(radius);
        _sphereCollider.radius = radius;

        Invoke("DisableObject", disableTime);
    }

    public void DisableObject()
    {
        CancelInvoke();
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        ObjectPool.ReturnGameObjectToPool(gameObject);
    }

    public Vector3 ReturnPos()
    {
        return transform.position;
    }

    public Transform ReturnTransform()
    {
        return transform;
    }

    public Transform ReturnSightPoint()
    {
        return null;
    }
}
