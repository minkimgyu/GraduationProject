using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using DamageUtility;
using System;

abstract public class BaseFactory<T1, T2>
{
    protected JsonAssetGenerator _jsonAssetGenerator = new JsonAssetGenerator();

    public abstract void Initialize(T2 data);
    public virtual T1 Create() { return default; }

    public virtual T1 Create(
        Func<Vector3> ReturnPlayerPos, 
        Action<BaseWeapon.Name> OnWeaponProfileChangeRequested,
        Action<float> OnHpChangeRequested,
        Action OnDieRequested
    ) 
    { return default; }

    public virtual T1 Create(
        Func<ShopBlackboard> ReturnBlackboard
    )
    { return default; }
}

abstract public class WeaponFactory<T1> : BaseFactory<BaseWeapon, WeaponFactoryData>
{
    protected T1 _data;
    protected GameObject _prefab;

    public override void Initialize(WeaponFactoryData data)
    {
        TextAsset asset = data._jsonAsset;
        _data = _jsonAssetGenerator.JsonToObject<T1>(asset);
        _prefab = data._prefab;
    }
}

[System.Serializable]
public class BaseFactoryData
{
    public GameObject _prefab;
    public TextAsset _jsonAsset;
}


[System.Serializable]
public class WeaponFactoryData : BaseFactoryData
{
    public List<RecoilData> _recoilDatas;
}

[System.Serializable]
public struct RecoilData
{
    public BaseWeapon.EventType _type;
    public TextAsset _asset;
}

[System.Serializable]
public struct SerializableVector3
{
    public float x;
    public float y;
    public float z;

    [JsonIgnore]
    public Vector3 V3 { get { return new Vector3(x, y, z); } }
}

public class WeaponPlant : MonoBehaviour
{
    Dictionary<BaseWeapon.Name, BaseFactory<BaseWeapon, WeaponFactoryData>> _weaponFactories;
    [SerializeField] WeaponDataDictionary _weaponDatas; // 무기 prefab을 모아서 넣어준다.

    private void Awake()
    {
        _weaponFactories = new Dictionary<BaseWeapon.Name, BaseFactory<BaseWeapon, WeaponFactoryData>>();
        Initialize();
    }

    private void Initialize()
    {
        // 여기서 추가
        _weaponFactories[BaseWeapon.Name.AR] = new AutomaticGunFactory();
        _weaponFactories[BaseWeapon.Name.AK] = new AutomaticGunFactory();
        _weaponFactories[BaseWeapon.Name.LMG] = new AutomaticGunFactory();
        _weaponFactories[BaseWeapon.Name.Knife] = new KnifeFactory();

        _weaponFactories[BaseWeapon.Name.Pistol] = new ClassicFactory();
        _weaponFactories[BaseWeapon.Name.Shotgun] = new BuckyFactory();
        _weaponFactories[BaseWeapon.Name.AutoShotgun] = new JudgeFactory();
        _weaponFactories[BaseWeapon.Name.Sniper] = new OperatorFactory();
        _weaponFactories[BaseWeapon.Name.SMG] = new StingerFactory();
        _weaponFactories[BaseWeapon.Name.DMR] = new GuardianFactory();

        foreach (var item in _weaponFactories) item.Value.Initialize(_weaponDatas[item.Key]);
    }

    public BaseWeapon Create(BaseWeapon.Name name)
    {
        return _weaponFactories[name].Create();
    }
}
