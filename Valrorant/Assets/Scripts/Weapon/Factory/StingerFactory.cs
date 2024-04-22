using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

[System.Serializable]
public class StingerData : GunData
{
    public float autoFireInterval;
    public float burstFireInterval;

    public float penetratePower;
    public float recoveryDuration;

    public float zoomDelay;
    public int burstFireCntInOneAction;
    public int mainFireCnt;

    public float burstAttackRecoverDuration;
    public float bulletSpreadPowerDecreaseRatio;
    public float recoilRatio;

    public float subActionDelay;
    public float zoomDuration;


    public float normalFieldOfView;
    public float zoomFieldOfView;
    public Vector3 zoomCameraPosition;

    public Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary;
    public WeightApplier weightApplier;
}

public class StingerFactory : WeaponFactory<StingerData>
{
    RecoilMapData _mainMapData;
    RecoilRangeData _subRangeData;

    public override void Initialize(WeaponFactoryData data)
    {
        base.Initialize(data);
        _mainMapData = _jsonAssetGenerator.JsonToObject<RecoilMapData>(data._recoilDatas.Find(x => x._type == BaseWeapon.EventType.Main)._asset);
        _subRangeData = _jsonAssetGenerator.JsonToObject<RecoilRangeData>(data._recoilDatas.Find(x => x._type == BaseWeapon.EventType.Sub)._asset);
    }

    public override BaseWeapon Create()
    {
        BaseWeapon weapon = Object.Instantiate(_prefab).GetComponent<BaseWeapon>();

        weapon.SetDefaultValue(); // 기본 값들을 초기화 해준다.
        weapon.Initialize(_data, _mainMapData, _subRangeData); // 무기를 초기화시킨다.
        weapon.MatchStrategy(); // Strategy를 매칭해준다.
        return weapon;
    }
}
