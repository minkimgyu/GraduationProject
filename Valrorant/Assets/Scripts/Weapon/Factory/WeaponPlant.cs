using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseFactory<T1, T2>
{
    protected JsonAssetGenerator _jsonAssetGenerator = new JsonAssetGenerator();

    public abstract void Initialize(T2 data);
    public abstract T1 Create();
}

abstract public class WeaponFactory<T1> : BaseFactory<BaseWeapon, WeaponFactoryData>
{
    protected T1 _data;
    protected GameObject _prefab;

    public override void Initialize(WeaponFactoryData data)
    {
        TextAsset asset = data._weaponJsonAsset;
        _data = _jsonAssetGenerator.JsonToObject<T1>(asset);
        _prefab = data._prefab;
    }
}

[System.Serializable]
public struct WeaponFactoryData
{
    public GameObject _prefab;
    public TextAsset _weaponJsonAsset;
    public List<RecoilData> _recoilDatas;
}

[System.Serializable]
public struct RecoilData
{
    public BaseWeapon.EventType _type;
    public TextAsset _asset;
}

public class WeaponPlant : MonoBehaviour
{
    Dictionary<BaseWeapon.Name, BaseFactory<BaseWeapon, WeaponFactoryData>> _weaponFactories;
    [SerializeField] WeaponDataDictionary _weaponDatas; // 무기 prefab을 모아서 넣어준다.

    //[SerializeField] BuckyData data1;
    //[SerializeField] AutomaticGunData data2;
    //[SerializeField] ClassicData data3;
    //[SerializeField] StingerData data4;
    //[SerializeField] OperatorData data5;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        // 여기서 추가
        _weaponFactories[BaseWeapon.Name.AR] = new AutomaticGunFactory();
        _weaponFactories[BaseWeapon.Name.AK] = new AutomaticGunFactory();
        _weaponFactories[BaseWeapon.Name.LMG] = new AutomaticGunFactory(); 

        _weaponFactories[BaseWeapon.Name.Pistol] = new ClassicFactory(); 
        _weaponFactories[BaseWeapon.Name.Shotgun] = new BuckyFactory(); 
        _weaponFactories[BaseWeapon.Name.Sniper] = new OperatorFactory(); 
        _weaponFactories[BaseWeapon.Name.SMG] = new StingerFactory(); 

        foreach (var item in _weaponFactories) item.Value.Initialize(_weaponDatas[item.Key]);
    }

    public BaseWeapon CreateWeapon(BaseWeapon.Name name)
    {
        return _weaponFactories[name].Create();
    }
}
