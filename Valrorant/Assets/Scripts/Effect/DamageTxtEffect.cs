using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class DamageTxtEffect : BaseEffect
{
    Vector3 finalPoint;

    [SerializeField]
    float upPoint = 5;

    [SerializeField]
    TMP_Text _text;

    public override void Initialize(Vector3 hitPosition, Vector3 hitNormal, Quaternion holeRotation, float damage)
    {
        _text.text = damage.ToString();

        transform.position = hitPosition;
        transform.rotation = holeRotation * transform.rotation;
        finalPoint = new Vector3(transform.position.x, transform.position.y + upPoint, transform.position.z);
    }

    public override void PlayEffect()
    {
        _timer.Start(_duration);
    }

    protected override void OnUpdate()
    {
        _timer.Update();

        if (_timer.IsRunning)
        {
            Ascend(_timer.Ratio);
            return;
        }

        if (_timer.IsFinish)
        {
            DisableObject();
        }
    }

    void Ascend(float progress)
    {
        transform.position = Vector3.Lerp(transform.position, finalPoint, progress);
    }
}
