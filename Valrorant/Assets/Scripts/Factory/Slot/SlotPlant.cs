using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class SlotFactory<T1> : BaseFactory<ItemSlot, SlotFactoryData>
{
    protected T1 _data;
    protected GameObject _prefab;

    public override void Initialize(SlotFactoryData data)
    {
        TextAsset asset = data._jsonAsset;
        _data = _jsonAssetGenerator.JsonToObject<T1>(asset);
        _prefab = data._prefab;
    }
}

[System.Serializable]
public class SlotFactoryData : BaseFactoryData { }

public class SlotPlant : MonoBehaviour
{
    public enum Name
    {
        ReviveKit,
        AidKit,
        AmmoPack,

        Glock18,
        MP5,

        AKM,
        M416,
        Scout,

        Saiga,
        MK18,
        M249
    }

    Dictionary<Name, BaseFactory<ItemSlot, SlotFactoryData>> _slotFactories;
    [SerializeField] SlotDataDictionary _slotDatas; // 무기 prefab을 모아서 넣어준다.

    private void Awake()
    {
        _slotFactories = new Dictionary<Name, BaseFactory<ItemSlot, SlotFactoryData>>();
        Initialize();
    }

    private void Initialize()
    {
        _slotFactories[Name.ReviveKit] = new ReviveSlotFactory();
        _slotFactories[Name.AidKit] = new HealSlotFactory();
        _slotFactories[Name.AmmoPack] = new AmmoSlotFactory();

        _slotFactories[Name.Glock18] = new WeaponSlotFactory();
        _slotFactories[Name.MP5] = new WeaponSlotFactory();
        _slotFactories[Name.AKM] = new WeaponSlotFactory();
        _slotFactories[Name.M416] = new WeaponSlotFactory();
        _slotFactories[Name.Scout] = new WeaponSlotFactory();
        _slotFactories[Name.Saiga] = new WeaponSlotFactory();
        _slotFactories[Name.MK18] = new WeaponSlotFactory();
        _slotFactories[Name.M249] = new WeaponSlotFactory();

        foreach (var item in _slotFactories) item.Value.Initialize(_slotDatas[item.Key]);
    }

    public ItemSlot Create(Name name, Func<ShopBlackboard> ReturnBlackboard)
    {
        return _slotFactories[name].Create(ReturnBlackboard);
    }
}
