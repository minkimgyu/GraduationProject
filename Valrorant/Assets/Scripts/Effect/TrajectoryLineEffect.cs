using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryLineEffect : CoroutineEffect
{
    Vector3 _hitPosition;

    public override void Initialize(Vector3 hitPosition, Vector3 shootPosition)
    {
        transform.position = shootPosition;

        _hitPosition = hitPosition;
    }

    public override void PlayEffect()
    {
        StartCoroutine(Routine());
    }

    protected override IEnumerator Routine()
    {
        float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
        float increment = smoothness / duration; //The amount of change to apply.
        while (progress < 1)
        {
            transform.position = Vector3.Lerp(transform.position, _hitPosition, progress);
            progress += increment;
            yield return new WaitForSeconds(smoothness);
        }

        DisableObject();
    }
}
