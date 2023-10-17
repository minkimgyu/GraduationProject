using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Recoil/Map", fileName = "Map")]
public class RecoilMap : RecoilData
{
    [SerializeField]
    Vector2[] _recoilData; // 이 데이터를 받아서 초기화 시켜줘야함
    public Vector2[] RecoilData { get { return _recoilData; } }

    [SerializeField]
    int _repeatIndex = 0; // --> 이거는 recoilMap 안에 존재해야함
    public int RepeatIndex { get { return _repeatIndex; } }
}
