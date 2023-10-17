using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageTxtEffect : BaseEffect
{
    protected SmoothLerpRoutine runningRoutine;

    Vector3 finalPoint;

    [SerializeField]
    float upPoint = 15;

    TMP_Text _text;

    protected virtual void Awake()
    {
        _text = GetComponentInChildren<TMP_Text>();
        TryGetComponent(out runningRoutine);

        if (runningRoutine == null) return;

        runningRoutine.Initialize();
        runningRoutine.RoutineEnd += DisableObject;
        runningRoutine.RoutineAction += Ascend;
    }

    public override void Initialize(Vector3 hitPosition, Vector3 hitNormal, Quaternion holeRotation, float damage)
    {
        _text.text = damage.ToString();

        transform.position = hitPosition;
        transform.rotation = holeRotation * transform.rotation;
        finalPoint = new Vector3(transform.position.x, transform.position.y + upPoint, transform.position.z);
    }

    public override void PlayEffect()
    {
        if (runningRoutine == null) return;
        runningRoutine.StartRoutine();
    }

    void Ascend(float progress)
    {
        transform.position = Vector3.Lerp(transform.position, finalPoint, progress);
    }
}
