using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleDrawer : BaseDrawer
{
    [SerializeField] float _radius;
    [SerializeField] Vector3 _distrance;

    [SerializeField] float _maxSteps = 20;

    public override void ResetData(float radius, Vector3 distance)
    {
        _radius = radius;
        _distrance = distance;
    }

    protected override void DrawGizmo()
    {
        DrawCircle(transform.position + _distrance, _radius, _maxSteps);
    }

    Vector3 ReturnStartPoint(Vector3 position)
    {
        return new Vector3(_radius, 0, 0) + position;
    }

    void DrawCircle(Vector3 position, float radius, float maxSteps = 20)
    {
        Vector3 startPoint = ReturnStartPoint(position);
        for (var i = 0; i <= maxSteps; i++)
        {
            float progress = i / maxSteps;
            float radian = progress * 2 * Mathf.PI;
            Vector3 pos = new Vector3(radius * Mathf.Cos(radian), 0, radius * Mathf.Sin(radian));

            pos += position;

            Gizmos.DrawLine(startPoint, pos);
            startPoint = pos;
        }

        Gizmos.DrawLine(startPoint, ReturnStartPoint(position));
    }
}
