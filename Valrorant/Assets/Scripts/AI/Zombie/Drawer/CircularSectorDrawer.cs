using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularSectorDrawer : BaseDrawer
{
    [SerializeField] float _angle;
    [SerializeField] float _radius;
    [SerializeField] float _maxSteps = 20;

    public override void ResetData(float angle, float radius)
    {
        _angle = angle;
        _radius = radius;
    }

    protected override void DrawGizmo()
    {
        DrawWireArc(transform.position, transform.forward, _angle, _radius);
    }

    void DrawWireArc(Vector3 position, Vector3 dir, float anglesRange, float radius)
    {
        float srcAngles = GetAnglesFromDir(position, dir);
        Vector3 initialPos = position;
        Vector3 posA = initialPos;
        float stepAngles = anglesRange / _maxSteps;
        float angle = srcAngles - anglesRange / 2;
        for (var i = 0; i <= _maxSteps; i++)
        {
            float rad = Mathf.Deg2Rad * angle;
            Vector3 posB = initialPos;
            posB += new Vector3(radius * Mathf.Cos(rad), 0, radius * Mathf.Sin(rad));

            Gizmos.DrawLine(posA, posB);

            angle += stepAngles;
            posA = posB;
        }
        Gizmos.DrawLine(posA, initialPos);
    }

    float GetAnglesFromDir(Vector3 position, Vector3 dir)
    {
        Vector3 forwardLimitPos = position + dir;
        float srcAngles = Mathf.Rad2Deg * Mathf.Atan2(forwardLimitPos.z - position.z, forwardLimitPos.x - position.x);

        return srcAngles;
    }
}