using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Recoil/Range", fileName = "Range")]
public class RecoilRange : RecoilData
{
    [SerializeField]
    float _yUpPoint; // 이 데이터를 받아서 초기화 시켜줘야함

    public float YUpPoint { get { return _yUpPoint; } }
}
