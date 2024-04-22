using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

[System.Serializable]
public class BuckyData : GunData
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
    public Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary;
    public Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> subSingleAttackDamageDictionary;
}

public class BuckyFactory : WeaponFactory<BuckyData>
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

        weapon.SetDefaultValue(); // �⺻ ������ �ʱ�ȭ ���ش�.
        weapon.Initialize(_data, _mainRecoilData, _subRecoilData); // ���⸦ �ʱ�ȭ��Ų��.
        weapon.MatchStrategy(); // Strategy�� ��Ī���ش�.
        return weapon;
    }
}
