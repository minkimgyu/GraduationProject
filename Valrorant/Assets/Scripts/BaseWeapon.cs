using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ���, ���� ��� �پ��� ���� �߰�
abstract public class BaseWeapon : MonoBehaviour
{
    abstract public void Initialize(Transform holder, Transform cam);

    abstract public void Action();
}
