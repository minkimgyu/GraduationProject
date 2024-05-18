using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

[System.Serializable]
public class GuardianData : GunData
{
    public float fireIntervalWhenZoomIn;
    public float fireIntervalWhenZoomOut;

    public int fireCnt;

    public float penetratePower;
    public float recoveryDuration;
    public float zoomDelay;

    public float zoomDuration;
    public float normalFieldOfView;
    public float zoomFieldOfView;

    public WeightApplier mainWeightApplier;

    public float displacementSpreadMultiplyRatio;

    public SerializableVector3 cameraPositionWhenZoom;
    public Dictionary<HitArea, DistanceAreaData[]> damageDictionary;
}

public class GuardianFactory : WeaponFactory<GuardianData>
{
    RecoilRangeData _mainRecoilData;

    public override void Initialize(WeaponFactoryData data)
    {
        base.Initialize(data);
        _mainRecoilData = _jsonAssetGenerator.JsonToObject<RecoilRangeData>(data._recoilDatas.Find(x => x._type == BaseWeapon.EventType.Main)._asset);
    }

    public override BaseWeapon Create()
    {
        BaseWeapon weapon = Object.Instantiate(_prefab).GetComponent<BaseWeapon>();

        weapon.SetDefaultValue(); // 기본 값들을 초기화 해준다.
        weapon.Initialize(_data, _mainRecoilData); // 무기를 초기화시킨다.
        weapon.MatchStrategy(); // Strategy를 매칭해준다.
        return weapon;
    }
}
