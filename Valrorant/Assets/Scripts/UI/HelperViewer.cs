using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperViewer : MonoBehaviour
{
    Dictionary<CharacterPlant.Name, ProfileViewer> _helperProfileDictionary = new Dictionary<CharacterPlant.Name, ProfileViewer>();
    [SerializeField] ProfileViewer _viewer;
    [SerializeField] Transform _content;

    public void AddProfile(CharacterPlant.Name name)
    {
        ProfileViewer viewer = Instantiate(_viewer, _content);
        viewer.Initialize(name);
        _helperProfileDictionary.Add(name, viewer);
    }

    public ProfileViewer ReturnProfile(CharacterPlant.Name name)
    {
        return _helperProfileDictionary[name];
    }

    public void OnChangeHpViewer(CharacterPlant.Name name, float ratio)
    {
        _helperProfileDictionary[name].OnHpChangeRequested(ratio);
    }

    public void OnWeaponChange(CharacterPlant.Name name, BaseWeapon.Name weaponName)
    {
        _helperProfileDictionary[name].OnWeaponProfileChangeRequested(weaponName);
    }

    public void OnActiveProfileRequested(CharacterPlant.Name name)
    {
        _helperProfileDictionary[name].OnActiveProfileRequested();
    }

    public void OnDisableProfileRequested(CharacterPlant.Name name)
    {
        _helperProfileDictionary[name].OnDisableProfileRequested();
    }
}
