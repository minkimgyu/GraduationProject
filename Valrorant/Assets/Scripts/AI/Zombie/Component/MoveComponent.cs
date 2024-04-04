using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveComponent : MonoBehaviour
{
    Rigidbody _rigid;
    float _speed;

    Action<string, bool> ResetAnimatorValue;

    public void Initialize(float speed, Action<string, bool> ResetAnimatorValue)
    {
        _speed = speed;
        this.ResetAnimatorValue = ResetAnimatorValue;
        _rigid = GetComponent<Rigidbody>();
    }

    public Vector3 ReturnVelocity()
    {
        return _rigid.velocity;
    }

    public void Move(Vector3 dir)
    {
        _rigid.velocity = dir * _speed;
        ResetAnimatorValue?.Invoke("NowMove", true);
    }

    public void Stop()
    {
        _rigid.velocity = Vector3.zero;
        ResetAnimatorValue?.Invoke("NowMove", false);
    }
}