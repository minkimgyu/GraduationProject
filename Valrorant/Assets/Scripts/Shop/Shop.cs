using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Agent.Controller;
using UnityEngine.UI;
using TMPro;

public struct ShopBlackboard
{
    public ShopBlackboard(Action<Sprite, string, string> TurnOnPreview, Action TurnOffPreview, Action OnBuyAmmo,
        Action<BaseWeapon> OnBuyWeapon, Action<float> OnBuyHealpack, Func<BaseWeapon.Name, BaseWeapon> CreateWeapon,

        Action<CharacterPlant.Name, BaseWeapon> BuyWeaponToHelper,
        Action<CharacterPlant.Name> ReviveHelper,
        Func<int, bool> CanBuy,
        Action<int> Buy,

        Action<Sprite, Vector3, Action<CharacterPlant.Name>> OnDragStart, Action<Vector3> OnDrag, Action OnDragEnd)
    {
        this.TurnOnPreview = TurnOnPreview;
        this.TurnOffPreview = TurnOffPreview;

        this.OnBuyAmmo = OnBuyAmmo;
        this.OnBuyWeapon = OnBuyWeapon;
        this.OnBuyHealpack = OnBuyHealpack;
        this.CreateWeapon = CreateWeapon;

        this.BuyWeaponToHelper = BuyWeaponToHelper;
        this.ReviveHelper = ReviveHelper;
        this.CanBuy = CanBuy;
        this.Buy = Buy;

        this.OnDragStart = OnDragStart;
        this.OnDrag = OnDrag;
        this.OnDragEnd = OnDragEnd;
    }

    public Action<Sprite, string, string> TurnOnPreview { get; }
    public Action TurnOffPreview { get; }

    public Action OnBuyAmmo { get; }
    public Action<BaseWeapon> OnBuyWeapon { get; }
    public Action<float> OnBuyHealpack { get; }
    public Func<BaseWeapon.Name, BaseWeapon> CreateWeapon { get; }

    public Action<CharacterPlant.Name, BaseWeapon> BuyWeaponToHelper { get; }
    public Action<CharacterPlant.Name> ReviveHelper { get; }
    public Func<int, bool> CanBuy { get; }
    public Action<int> Buy { get; }

    public Action<Sprite, Vector3, Action<CharacterPlant.Name>> OnDragStart { get; }
    public Action<Vector3> OnDrag { get; }
    public Action OnDragEnd { get; }
}

public class Shop : MonoBehaviour
{
    protected Dictionary<EventType, BaseCommand> _commands = new Dictionary<EventType, BaseCommand>();

    [SerializeField] GameObject _panel;
    [SerializeField] Transform _slotContainer;
    [SerializeField] ItemSlotContainer _slotContainerPrefab;
    [SerializeField] ItemPreviewer _itemPreview;

    Dictionary<Category, List<SlotPlant.Name>> _itemDictionary;

    [SerializeField] ShopProfileViewer _profileViewerPrefab;
    [SerializeField] Transform _profileContent;
    Dictionary<Database.PersonName, ShopProfileViewer> _profileViewers = new Dictionary<Database.PersonName, ShopProfileViewer>();

    [SerializeField] DragSlot _dragSlot;
    [SerializeField] GameObject _helperViewerObj;
    [SerializeField] GameObject _weaponViewerObj;

    int _money = 0;
    [SerializeField] TMP_Text _moneyTxt;
    [SerializeField] TMP_Text _fieldMoneyTxt;

    public void AddMoney(int moneyPerOne)
    {
        _money += moneyPerOne;
        ResetMoney();
    }

    bool CanBuy(int cost)
    {
        return _money >= cost;
    }

    void Buy(int cost)
    {
        _money -= cost;
        ResetMoney();
    }

    void ResetMoney()
    {
        _moneyTxt.text = _money.ToString();
        _fieldMoneyTxt.text = _money.ToString();
    }

    public void AddProfileViewer(Database.PersonName name, out Action<BaseWeapon.Name, BaseWeapon.Type> AddWeaponPreview, out Action<BaseWeapon.Type> RemoveWeaponPreview)
    {
        ShopProfileViewer viewer = Instantiate(_profileViewerPrefab, _profileContent);

        viewer.Initialize(name, _dragSlot.CallShopingEvent);
        AddWeaponPreview = viewer.AddWeaponPreview;
        RemoveWeaponPreview = viewer.RemoveWeaponPreview;

        _profileViewers[name] = viewer;
    }

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
        BuyHealPack,

        BuyWeaponToHelper,
        ReviveHelper,
    }

    private void Start() => Initialize();

    public bool _isActive = false;

    private void TurnOnOffPanel()
    {
        if (_isActive == false) return;

        if (_panel.activeSelf == true)
        {
            _helperViewerObj.SetActive(true);
            _weaponViewerObj.SetActive(true);
            ActivatePanel(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            _helperViewerObj.SetActive(false);
            _weaponViewerObj.SetActive(false);
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
            CreateWeapon,

            (name, weapon) => { _commands[EventType.BuyWeaponToHelper].Execute(name, weapon); },
            (name) => { _commands[EventType.ReviveHelper].Execute(name); },
            CanBuy,
            Buy,

            _dragSlot.OnDragStart,
            _dragSlot.OnDraging,
            _dragSlot.OnDragEnd
        );

        return shopBlackboard;
    }

    Func<BaseWeapon.Name, BaseWeapon> CreateWeapon;

    private void Initialize()
    {
        ActivatePanel(false);

        _moneyTxt.text = _money.ToString();
        _fieldMoneyTxt.text = _money.ToString();

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
