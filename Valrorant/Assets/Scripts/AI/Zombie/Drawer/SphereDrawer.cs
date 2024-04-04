using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereDrawer : BaseDrawer
{
    [SerializeField] float _radius;
    public override void ResetData(float radius)
    {
        _radius = radius;
    }

    protected override void DrawGizmo()
    {
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
