using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Agent.Controller;

public struct ShopBlackboard
{
    public ShopBlackboard(Action<Sprite, string, string> TurnOnPreview, Action TurnOffPreview, Action OnBuyAmmo,
        Action<BaseWeapon> OnBuyWeapon, Action<float> OnBuyHealpack, Func<BaseWeapon.Name, BaseWeapon> CreateWeapon)
    {
        this.TurnOnPreview = TurnOnPreview;
        this.TurnOffPreview = TurnOffPreview;

        this.OnBuyAmmo = OnBuyAmmo;
        this.OnBuyWeapon = OnBuyWeapon;
        this.OnBuyHealpack = OnBuyHealpack;
        this.CreateWeapon = CreateWeapon;
    }

    public Action<Sprite, string, string> TurnOnPreview { get; }
    public Action TurnOffPreview { get; }

    public Action OnBuyAmmo { get; }
    public Action<BaseWeapon> OnBuyWeapon { get; }
    public Action<float> OnBuyHealpack { get; }
    public Func<BaseWeapon.Name, BaseWeapon> CreateWeapon { get; }
}

public class Shop : MonoBehaviour
{
    protected Dictionary<EventType, BaseCommand> _commands = new Dictionary<EventType, BaseCommand>();

    [SerializeField] GameObject _panel;
    [SerializeField] Transform _slotContainer;
    [SerializeField] ItemSlotContainer _slotContainerPrefab;
    [SerializeField] ItemPreviewer _itemPreview;

    Dictionary<Category, List<SlotPlant.Name>> _itemDictionary;

    public enum Category
    {
        Comsumable,
        Sub,
        Main,
        Special
    }

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
            (model, title, info) => _itemPreview.TurnOnPreview(model, title, info),
            _itemPreview.TurnOffPreview,

            _commands[EventType.BuyAmmo].Execute,
            (weapon) => { _commands[EventType.BuyWeapon].Execute(weapon); },
            (hp) => { _commands[EventType.BuyHealPack].Execute(hp); },

            CreateWeapon
        );

        return shopBlackboard;
    }

    Func<BaseWeapon.Name, BaseWeapon> CreateWeapon;

    private void Initialize()
    {
        ActivatePanel(false);
        _itemPreview.TurnOffPreview();
        CreateWeapon = FindObjectOfType<WeaponPlant>().Create;

        InputHandler.AddInputEvent(InputHandler.Type.Shop, new Command(TurnOnOffPanel));

        JsonAssetGenerator generator = new JsonAssetGenerator();


        _itemDictionary = new Dictionary<Category, List<SlotPlant.Name>>()
        {
            {   Category.Comsumable, new List<SlotPlant.Name>()
                {
                    SlotPlant.Name.AidKit,
                    SlotPlant.Name.AmmoPack,
                    SlotPlant.Name.ReviveKit
                }
            },
            {   Category.Sub, new List<SlotPlant.Name>()
                {
                    SlotPlant.Name.Glock18,
                    SlotPlant.Name.MP5,
                } 
            },
            {   Category.Main, new List<SlotPlant.Name>()
                {
                    SlotPlant.Name.AKM,
                    SlotPlant.Name.M416,
                    SlotPlant.Name.Scout
                } 
            },
            {   Category.Special, new List<SlotPlant.Name>()
                {
                    SlotPlant.Name.Saiga,
                    SlotPlant.Name.MK18,
                    SlotPlant.Name.M249
                } 
            }
        };

        foreach (var item in _itemDictionary)
        {
            ItemSlotContainer slotContainer = Instantiate(_slotContainerPrefab, _slotContainer);
            slotContainer.Initialize(item.Key.ToString(), item.Value, ReturnBlackboard);
        }
    }
}
