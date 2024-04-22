using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

[System.Serializable]
public class WeaponData
{
    public float range;
    public BaseWeapon.Name weaponName;
    public BaseWeapon.Type weaponType;

    public float equipFinishTime;
    public float weaponWeight;
}


[System.Serializable]
public class GunData : WeaponData
{
    public int maxAmmoCountInMagazine;
    public int maxAmmoCountsInPossession;

    public float reloadFinishDuration;
    public float reloadExitDuration;
}

[System.Serializable]
public class AutomaticGunData : GunData
{
    public float fireIntervalWhenZoomIn;
    public float fireIntervalWhenZoomOut;

    public int fireCnt;
    public float penetratePower;

    public float recoveryDuration;

    public float recoilRatioWhenZoomIn;
    public float recoilRatioWhenZoomOut;

    public float zoomDelay;
    public float displacementSpreadMultiplyRatio;
    public float zoomDuration;
    public float normalFieldOfView;
    public float zoomFieldOfView;
    public Vector3 cameraPositionWhenZoom;

    public Dictionary<DistanceAreaData.HitArea, DistanceAreaData[]> damageDictionary;
}

public class AutomaticGunFactory : WeaponFactory<AutomaticGunData>
{
    RecoilMapData _mapData;

    public override void Initialize(WeaponFactoryData data)
    {
        base.Initialize(data);
        _mapData = _jsonAssetGenerator.JsonToObject<RecoilMapData>(data._recoilDatas.Find(x => x._type == BaseWeapon.EventType.Main)._asset);
    }

    public override BaseWeapon Create()
    {
        BaseWeapon weapon = Object.Instantiate(_prefab).GetComponent<BaseWeapon>();

        weapon.SetDefaultValue(); // 기본 값들을 초기화 해준다.
        weapon.Initialize(_data, _mapData); // 무기를 초기화시킨다.
        weapon.MatchStrategy(); // Strategy를 매칭해준다.
        return weapon;
    }
}
