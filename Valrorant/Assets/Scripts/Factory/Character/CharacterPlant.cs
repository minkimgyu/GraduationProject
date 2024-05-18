using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class CharacterFactory<T1> : BaseFactory<Transform, CharacterFactoryData>
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
public class CharacterFactoryData : BaseFactoryData
{
}

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

    Dictionary<Name, BaseFactory<Transform, CharacterFactoryData>> _characterFactories;
    [SerializeField] CharacterDataDictionary _characterDatas; // ���� prefab�� ��Ƽ� �־��ش�.

    private void Awake()
    {
        _characterFactories = new Dictionary<Name, BaseFactory<Transform, CharacterFactoryData>>();
        Initialize();
    }

    private void Initialize()
    {
        // ���⼭ �߰�
        _characterFactories[Name.Player] = new PlayerFactory();
        _characterFactories[Name.Zombie] = new ZombieFactory();

        _characterFactories[Name.Oryx] = new HelperFactory();
        _characterFactories[Name.Rook] = new HelperFactory();
        _characterFactories[Name.Warden] = new HelperFactory();

        foreach (var item in _characterFactories) item.Value.Initialize(_characterDatas[item.Key]);
    }

    public Transform Create(Name name)
    {
        return _characterFactories[name].Create();
    }
}
