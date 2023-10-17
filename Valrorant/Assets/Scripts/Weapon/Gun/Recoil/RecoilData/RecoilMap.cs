using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Recoil/Map", fileName = "Map")]
public class RecoilMap : RecoilData
{
    [SerializeField]
    Vector2[] _recoilData; // �� �����͸� �޾Ƽ� �ʱ�ȭ ���������
    public Vector2[] RecoilData { get { return _recoilData; } }

    [SerializeField]
    int _repeatIndex = 0; // --> �̰Ŵ� recoilMap �ȿ� �����ؾ���
    public int RepeatIndex { get { return _repeatIndex; } }
}
