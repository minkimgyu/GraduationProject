using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileViewer : MonoBehaviour
{
    [SerializeField] Image _profile;
    [SerializeField] TMP_Text _name;

    [SerializeField] Image _equipedWeaponImg;
    [SerializeField] HpBarViewer _hpBarViewer;

    float _dieAlphaValue = 50f/255f;
    float _aliveAlphaValue = 255f/255f;

    T ParseEnum<T>(string name)
    {
        return (T)System.Enum.Parse(typeof(T), name);
    }

    public void OnHpChangeRequested(float ratio)
    {
        _hpBarViewer.OnChangeHpViewer(ratio);
    }

    public void Initialize(CharacterPlant.Name name)
    {
        Database.PersonName helperName = ParseEnum<Database.PersonName>(name.ToString());
        _profile.sprite = Database.ReturnProfile(helperName);
        _name.text = name.ToString();
    }

    public void OnWeaponProfileChangeRequested(BaseWeapon.Name name)
    {
        Debug.Log(name.ToString() + "Icon");
        Database.IconName iconName = ParseEnum<Database.IconName>(name.ToString() + "Icon");
        _equipedWeaponImg.sprite = Database.ReturnIcon(iconName);
    }

    public void OnActiveProfileRequested()
    {
        _profile.color = new Color(_profile.color.r, _profile.color.g, _profile.color.b, _aliveAlphaValue);
        _name.color = new Color(_name.color.r, _name.color.g, _name.color.b, _aliveAlphaValue);
        _equipedWeaponImg.color = new Color(_equipedWeaponImg.color.r, _equipedWeaponImg.color.g, _equipedWeaponImg.color.b, _aliveAlphaValue);

        _hpBarViewer.OnReviveRequested();
    }

    public void OnDisableProfileRequested()
    {
        _profile.color = new Color(_profile.color.r, _profile.color.g, _profile.color.b, _dieAlphaValue);
        _name.color = new Color(_name.color.r, _name.color.g, _name.color.b, _dieAlphaValue);
        _equipedWeaponImg.color = new Color(_equipedWeaponImg.color.r, _equipedWeaponImg.color.g, _equipedWeaponImg.color.b, _dieAlphaValue);

        _hpBarViewer.OnDieRequested();
    }
}
