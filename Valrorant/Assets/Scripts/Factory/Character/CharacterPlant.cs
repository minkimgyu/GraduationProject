using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class CharacterFactory<T1> : BaseFactory<GameObject, CharacterFactoryData>
{
    protected T1 _data;
    protected GameObject _prefab;

    public override void Initialize(CharacterFactoryData data)
    {
        TextAsset asset = data._jsonAsset;
        _data = _jsonAssetGenerator.JsonToObject<T1>(asset);
        _prefab = data._prefab;
    }
}

[System.Serializable]
public class CharacterFactoryData : BaseFactoryData { }

public class CharacterPlant : MonoBehaviour
{
    public enum Name
    {
        Zombie,
        Player,

        Oryx,
        Rook,
        Warden
    }

    Dictionary<Name, BaseFactory<GameObject, CharacterFactoryData>> _characterFactories;
    [SerializeField] CharacterDataDictionary _characterDatas; // 무기 prefab을 모아서 넣어준다.

    private void Awake()
    {
        _characterFactories = new Dictionary<Name, BaseFactory<GameObject, CharacterFactoryData>>();
        Initialize();
    }

    private void Initialize()
    {
        // 여기서 추가
        _characterFactories[Name.Player] = new PlayerFactory();
        _characterFactories[Name.Zombie] = new ZombieFactory();

        _characterFactories[Name.Oryx] = new HelperFactory();
        _characterFactories[Name.Rook] = new HelperFactory();
        _characterFactories[Name.Warden] = new HelperFactory();

        foreach (var item in _characterFactories) item.Value.Initialize(_characterDatas[item.Key]);
    }

    public GameObject Create(Name name, Vector3 pos)
    {
        return _characterFactories[name].Create(pos);
    }

    public GameObject Create(Name name, Vector3 pos, Func<Vector3> ReturnPlayerPos, Action<BaseWeapon.Name> OnWeaponProfileChangeRequested, 
        Action<float> OnHpChangeRequested, Action OnDieRequested)
    {
        return _characterFactories[name].Create(pos, ReturnPlayerPos, OnWeaponProfileChangeRequested, OnHpChangeRequested, OnDieRequested);
    }
}
