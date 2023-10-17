using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Recoil/Range", fileName = "Range")]
public class RecoilRange : RecoilData
{
    [SerializeField]
    float _yUpPoint; // �� �����͸� �޾Ƽ� �ʱ�ȭ ���������

    public float YUpPoint { get { return _yUpPoint; } }
}
