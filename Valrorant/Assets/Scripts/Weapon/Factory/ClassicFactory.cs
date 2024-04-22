using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

[System.Serializable]
public class ClassicData : GunData
{
    public float mainShootInterval;
    public float subShootInterval;

    public int mainFireCnt;
    public int subFireCnt;

    public float recoveryDuration;

    public float penetratePower;

    public int subActionPelletCount;
    public float subActionSpreadOffset;

    public WeightApplier mainWeightApplier;
    public WeightApplier subWeightApplier;

    public float displacementSpreadMultiplyRatio;
    public int subAttackBulletCounts;
    public float spreadOffset;
    public float mainActionDelay;
    public float subActionDelay;

    public float bulletSpreadPowerRatio;
    public float bulletSpreadPowerDecreaseRatio;

    public Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary;
}

public class ClassicFactory : WeaponFactory<ClassicData>
{
    RecoilRangeData _mainRecoilData;
    RecoilRangeData _subRecoilData;

    public override void Initialize(WeaponFactoryData data)
    {
        base.Initialize(data);
        _mainRecoilData = _jsonAssetGenerator.JsonToObject<RecoilRangeData>(data._recoilDatas.Find(x => x._type == BaseWeapon.EventType.Main)._asset);
        _subRecoilData = _jsonAssetGenerator.JsonToObject<RecoilRangeData>(data._recoilDatas.Find(x => x._type == BaseWeapon.EventType.Sub)._asset);
    }

    public override BaseWeapon Create()
    {
        BaseWeapon weapon = Object.Instantiate(_prefab).GetComponent<BaseWeapon>();

        weapon.SetDefaultValue(); // 기본 값들을 초기화 해준다.
        weapon.Initialize(_data, _mainRecoilData, _subRecoilData); // 무기를 초기화시킨다.
        weapon.MatchStrategy(); // Strategy를 매칭해준다.
        return weapon;
    }
}
