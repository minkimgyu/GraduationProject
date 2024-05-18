using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Agent.Controller;

public struct ShopBlackboard
{
    public ShopBlackboard(Action OnBuyAmmo, Action<BaseWeapon> OnBuyWeapon, Action<float, float> OnBuyHealpack)
    {
        this.OnBuyAmmo = OnBuyAmmo;
        this.OnBuyWeapon = OnBuyWeapon;
        this.OnBuyHealpack = OnBuyHealpack;
    }

    public Action OnBuyAmmo { get; }
    public Action<BaseWeapon> OnBuyWeapon { get; }
    public Action<float, float> OnBuyHealpack { get; }
}

public class Shop : MonoBehaviour
{
    protected Dictionary<EventType, BaseCommand> _commands = new Dictionary<EventType, BaseCommand>();

    [SerializeField] GameObject _panel;
    [SerializeField] Transform _slotContainer;
    [SerializeField] ItemSlotContainer _slotContainerPrefab;
    [SerializeField] ItemPreviewer _itemPreview;

    Dictionary<Tuple<string, ItemSlot.Type>, List<ItemData>> _itemDictionary;

    public Action<BaseWeapon> AddWeapon;

    public enum EventType
    {
        BuyWeapon,
        BuyAmmo,
        BuyHealPack
    }

    private void Start() => Initialize();

    private void TurnOnOffPanel()
    {
        if (_panel.activeSelf == true)
        {
            ActivatePanel(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            ActivatePanel(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void ActivatePanel(bool activate)
    {
        _panel.SetActive(activate);
    }

    public void AddEvent(EventType type, BaseCommand command)
    {
        _commands.Add(type, command);
    }

    public void RemoveEvent(EventType type)
    {
        _commands.Remove(type);
    }

    ShopBlackboard ReturnBlackboard()
    {
        ShopBlackboard shopBlackboard = new ShopBlackboard(
             _commands[EventType.BuyAmmo].Execute,
             (weapon) => _commands[EventType.BuyWeapon].Execute(weapon),
             (hp, armor) => _commands[EventType.BuyHealPack].Execute(hp, armor)
        );

        return shopBlackboard;
    }

    private void Initialize()
    {
        ActivatePanel(false);
        _itemPreview.TurnOffPreview();

        InputHandler.AddInputEvent(InputHandler.Type.Shop, new Command(TurnOnOffPanel));

        _itemDictionary = new Dictionary<Tuple<string, ItemSlot.Type>, List<ItemData>>()
        {
            { new Tuple<string, ItemSlot.Type>("Ammo", ItemSlot.Type.Ammo), new List<ItemData>()
                {
                    new AmmoItemData(ItemData.Name.Ammo, 300, "d", 
                    Database.ReturnIcon(Database.IconName.AmmoIcon), 
                    Database.ReturnPreview(Database.IconName.AmmoIcon)),
                }
            },

            { new Tuple<string, ItemSlot.Type>("Consumable", ItemSlot.Type.Consumable), new List<ItemData>()
                {
                    new HealItemData(ItemData.Name.AidKit, 100, 200, 300, "d", 
                    Database.ReturnIcon(Database.IconName.AidKitIcon), 
                    Database.ReturnPreview(Database.IconName.AidKitIcon))
                } 
            },

            {new Tuple<string, ItemSlot.Type>("Sub", ItemSlot.Type.Weapon), new List<ItemData>()
                { 
                    new WeaponItemData(ItemData.Name.Glock18, BaseWeapon.Name.Pistol, 300, "d", 
                    Database.ReturnIcon(Database.IconName.PistolIcon),
                    Database.ReturnPreview(Database.IconName.PistolIcon)),

                    new WeaponItemData(ItemData.Name.MP5, BaseWeapon.Name.SMG, 300, "d", 
                    Database.ReturnIcon(Database.IconName.SMGIcon),
                    Database.ReturnPreview(Database.IconName.SMGIcon)) 
                } 
            },

            {new Tuple<string, ItemSlot.Type>("Main", ItemSlot.Type.Weapon), new List<ItemData>()
                { 
                    new WeaponItemData(ItemData.Name.AKM, BaseWeapon.Name.AK, 300, "d", 
                    Database.ReturnIcon(Database.IconName.AKIcon),
                    Database.ReturnPreview(Database.IconName.AKIcon)), 

                    new WeaponItemData(ItemData.Name.M416, BaseWeapon.Name.AR, 300, "d", 
                    Database.ReturnIcon(Database.IconName.ARIcon),
                    Database.ReturnPreview(Database.IconName.ARIcon)), 

                    new WeaponItemData(ItemData.Name.Scout, BaseWeapon.Name.Sniper, 300, "d", 
                    Database.ReturnIcon(Database.IconName.SniperIcon),
                    Database.ReturnPreview(Database.IconName.SniperIcon)),
                } 
            },

            {new Tuple<string, ItemSlot.Type>("Special", ItemSlot.Type.Weapon), new List<ItemData>()
                { 
                    new WeaponItemData(ItemData.Name.Saiga, BaseWeapon.Name.AutoShotgun, 300, "d", 
                    Database.ReturnIcon(Database.IconName.AutoShotgunIcon),
                    Database.ReturnPreview(Database.IconName.AutoShotgunIcon)), 

                    new WeaponItemData(ItemData.Name.MK18, BaseWeapon.Name.DMR, 300, "d", 
                    Database.ReturnIcon(Database.IconName.DMRIcon),
                    Database.ReturnPreview(Database.IconName.DMRIcon)),

                    new WeaponItemData(ItemData.Name.M249, BaseWeapon.Name.LMG, 300, "d",
                    Database.ReturnIcon(Database.IconName.LMGIcon),
                    Database.ReturnPreview(Database.IconName.LMGIcon))
                } 
            },
        };

        foreach (var item in _itemDictionary)
        {
            ItemSlotContainer slotContainer = Instantiate(_slotContainerPrefab, _slotContainer);
            slotContainer.Initialize(item.Key.Item1, item.Key.Item2, item.Value, ReturnBlackboard,
               _itemPreview.TurnOffPreview,
               (model, title, info) => _itemPreview.TurnOnPreview(model, title, info)
            );
        }
    }
}
