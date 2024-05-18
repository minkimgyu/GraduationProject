using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

[System.Serializable]
public class JudgeData : GunData
{
    public float mainFireInterval;
    public float subFireInterval;

    public int mainFireCnt;
    public int subFireCnt;

    public float recoveryDuration;

    public float penetratePower;

    public int pelletCount;
    public float spreadOffset;

    public float bulletSpreadPowerDecreaseRatio;
    public float reloadBeforeDuration;
    public float frontDistance;

    public string explosionEffectName;
    public int subActionPelletCount;
    public float subScatterActionBulletSpreadPowerDecreaseRatio;
    public float subSingleActionBulletSpreadPowerDecreaseRatio;
    public float subActionSpreadOffset;
    public float subActionSinglePenetratePower;
    public float subActionScatterPenetratePower;

    public float findRange;
    public Dictionary<HitArea, DistanceAreaData[]> damageDictionary;
    public Dictionary<HitArea, DistanceAreaData[]> subSingleAttackDamageDictionary;
}

public class JudgeFactory : WeaponFactory<JudgeData>
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
