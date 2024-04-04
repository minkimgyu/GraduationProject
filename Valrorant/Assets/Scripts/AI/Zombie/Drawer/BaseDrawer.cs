using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseDrawer : MonoBehaviour
{
    [SerializeField] protected Color color;

    protected abstract void DrawGizmo();

    public virtual void ResetData(float radius) { }
    public virtual void ResetData(float angle, float radius) { }
    public virtual void ResetData(float radius, Vector3 distance) { }

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        DrawGizmo();
    }
}
