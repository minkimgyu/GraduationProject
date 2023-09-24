using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 드랍, 무게 등등 다양한 변수 추가
abstract public class BaseWeapon : MonoBehaviour
{
    abstract public void Initialize(Transform holder, Transform cam);

    abstract public void Action();
}
