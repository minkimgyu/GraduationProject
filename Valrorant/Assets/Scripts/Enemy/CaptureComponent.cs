using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CaptureComponent : MonoBehaviour
{
    float _captureAngle = 90;

    [SerializeField] WireDrawer _drawer;

    [SerializeField] float _captureRadius;
    public float CaptureRadius { get { return _captureRadius; } }

    [SerializeField] float _raycastOffset = 1;

    float RaycastDistance { get { return _captureRadius + _raycastOffset; } }

    List<GameObject> _capturedEnemies = new List<GameObject>();

    Transform _capturedEnemy;
    public Transform CapturedEnemy { get { return _capturedEnemy; } }

    SphereCollider _captureCollider;

    [SerializeField] Transform _sightPoint;

    public bool IsCapturedListEmpty() { return _capturedEnemies.Count == 0; }

    private void Start()
    {
        _captureCollider = GetComponent<SphereCollider>();
        _captureCollider.radius = _captureRadius;
    }

    bool RayToTarget(Transform target)
    {
        Vector3 dir = (new Vector3(target.position.x, _sightPoint.position.y, target.position.z) - _sightPoint.position).normalized;

        RaycastHit hit;
        Physics.Raycast(_sightPoint.position, dir, out hit, RaycastDistance);
        Debug.DrawRay(_sightPoint.position, dir * RaycastDistance, Color.yellow);

        if (hit.transform == target) return true;
        else return false;
    }

    public bool IsTargetInSight()
    {
        for (int i = 0; i < _capturedEnemies.Count; i++)
        {
            if (_capturedEnemies[i] == null) continue;

            float angle = ReturnAngleBetween(_capturedEnemies[i].transform.position);
            if (angle <= _captureAngle / 2 && angle >= -_captureAngle / 2)
            {
                bool result = RayToTarget(_capturedEnemies[i].transform);
                if (result == true)
                {
                    _capturedEnemy = _capturedEnemies[i].transform;
                    return true;
                }
            }
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_capturedEnemies.Contains(other.gameObject) == true) return;
        if (other.gameObject.CompareTag("Player") == false) return;

       _capturedEnemies.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_capturedEnemies.Contains(other.gameObject) == false) return;
        if (other.gameObject.CompareTag("Player") == false) return;

        _capturedEnemies.Remove(other.gameObject);
    }

    float ReturnAngleBetween(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position).normalized;
        float angle= Vector3.Angle(transform.forward, dir);
        return angle;
    }

    private void OnValidate()
    {
        if (_drawer == null) return;
        _drawer.SetCaptureValue(_captureAngle, _captureRadius);
    }
}