using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;
using System;
using Random = UnityEngine.Random;

public class Commander : MonoBehaviour
{
    Dictionary<CharacterPlant.Name, ICommandListener> _listeners = new Dictionary<CharacterPlant.Name, ICommandListener>();
    CharacterPlant.Name[] helperNames = { CharacterPlant.Name.Oryx, CharacterPlant.Name.Rook, CharacterPlant.Name.Warden };

    float _startRange;

    public void Initialize(Func<Vector3> ReturnPlayerPos, float startRange)
    {
        _startRange = startRange;

        CharacterPlant plant = FindObjectOfType<CharacterPlant>();
        HelperViewer viewer = FindObjectOfType<HelperViewer>();
        Player player = FindObjectOfType<Player>();

        for (int i = 0; i < helperNames.Length; i++)
        {
            viewer.AddProfile(helperNames[i]);
            ProfileViewer profile = viewer.ReturnProfile(helperNames[i]);

            Vector2 startPos = Random.insideUnitCircle * startRange;
            Transform helper = plant.Create(helperNames[i], ReturnPlayerPos, profile.OnWeaponProfileChangeRequested, 
                profile.OnHpChangeRequested, () => { profile.OnDisableProfileRequested(); _listeners.Remove(helperNames[i]); ResetFormationData(); });

            helper.position = new Vector3(transform.position.x + startPos.y, transform.position.y, transform.position.z + startPos.x);
            helper.rotation = Quaternion.identity;

            ICommandListener listener = helper.GetComponent<ICommandListener>();
            _listeners.Add(helperNames[i], listener);

        }

        ResetFormationData();
    }

    public void ReviveListener(CharacterPlant.Name name)
    {
        CharacterPlant plant = FindObjectOfType<CharacterPlant>();
        HelperViewer viewer = FindObjectOfType<HelperViewer>();
        Player player = FindObjectOfType<Player>();
        ProfileViewer profile = viewer.ReturnProfile(name);

        profile.OnActiveProfileRequested(); // ��Ȱ��ȭ

        Vector2 startPos = Random.insideUnitCircle * _startRange;
        Transform helper = plant.Create(name, player.ReturnPos, profile.OnWeaponProfileChangeRequested,
            profile.OnHpChangeRequested, () => { profile.OnDisableProfileRequested(); _listeners.Remove(name); ResetFormationData(); });

        helper.position = new Vector3(transform.position.x + startPos.y, transform.position.y, transform.position.z + startPos.x);
        helper.rotation = Quaternion.identity;

        ICommandListener listener = helper.GetComponent<ICommandListener>();
        _listeners.Add(name, listener);

        ResetFormationData();
    }

    public void BuyWeaponToListener(CharacterPlant.Name name, BaseWeapon weapon)
    {
        _listeners[name].ReceiveWeapon(weapon);
    }

    public void HealListeners(float hpRatio)
    {
        foreach (var listener in _listeners)
        {
            listener.Value.Heal(hpRatio);
        }
    }

    public void RefillAmmoToListeners()
    {
        foreach (var listener in _listeners)
        {
            listener.Value.RefillAmmo();
        }
    }

    public void ResetFormationData()
    {
        int index = 0;
        foreach (var listener in _listeners)
        {
            FormationData data = new FormationData(index, _listeners.Count, _listeners);
            listener.Value.ResetFormationData(data);
            index++;
        }
    }

    public void BuildFormation()
    {
        foreach (var listener in _listeners)
        {
            listener.Value.GoToBuildFormationState();
        }
    }

    public void FreeRole()
    {
        foreach (var listener in _listeners)
        {
            listener.Value.GoToFreeRoleState();
        }
    }

    public void PickUpWeapon()
    {

    }

    public void SetPriorityTarget()
    {

    }
}
