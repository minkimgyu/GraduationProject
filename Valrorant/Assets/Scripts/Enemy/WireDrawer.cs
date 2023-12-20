using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireDrawer : MonoBehaviour
{
    float _captureAngle;

    float _captureRadius;

    float _stopDistance;

    public void SetCaptureValue(float captureAngle, float captureRadius)
    {
        _captureAngle = captureAngle;
        _captureRadius = captureRadius;
    }

    public void SetNavigationValue(float stopDistance)
    {
        _stopDistance = stopDistance;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        DrawWireArc(transform.position, transform.forward, _captureAngle, _captureRadius);

        Gizmos.color = Color.red;
        DrawWireArc(transform.position, transform.forward, _captureAngle, _stopDistance);
    }

    public void DrawWireArc(Vector3 position, Vector3 dir, float anglesRange, float radius, float maxSteps = 20)
    {
        float srcAngles = GetAnglesFromDir(position, dir);
        Vector3 initialPos = position;
        Vector3 posA = initialPos;
        float stepAngles = anglesRange / maxSteps;
        float angle = srcAngles - anglesRange / 2;
        for (var i = 0; i <= maxSteps; i++)
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
