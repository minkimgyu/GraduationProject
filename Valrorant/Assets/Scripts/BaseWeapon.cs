using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ���, ���� ��� �پ��� ���� �߰�
public abstract class BaseWeapon : MonoBehaviour
{
    protected int targetLayer;

    private void Awake()
    {
        targetLayer = LayerMask.GetMask("PenetrateTarget");
    }

    abstract public void Initialize(Transform holder, Transform cam);

    abstract public void Action();
}
