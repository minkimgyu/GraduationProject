using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

[System.Serializable]
public class OperatorData : GunData
{
    public float mainActionDelayWhenZoomIn;

    public float mainActionDelayWhenZoomOut;

    public int fireCnt;

    public float penetratePower;

    public float recoveryDuration;



    public float subActionDelay;


    public float mainActionbulletSpreadPowerRatio;



    public float meshDisableDelay;


    public float zoomDuration;


    public float normalFieldOfView;


    public float zoomFieldOfView;


    public float doubleZoomFieldOfView;


    public Vector3 zoomCameraPosition;


    public WeightApplier mainWeightApplier;

    public Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary;
}

public class OperatorFactory : WeaponFactory<OperatorData>
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
