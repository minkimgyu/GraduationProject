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

            Vector2 pos = Random.insideUnitCircle * startRange;
            GameObject helperObj = plant.Create(helperNames[i], new Vector3(pos.y, -0.5f, pos.x), ReturnPlayerPos, profile.OnWeaponProfileChangeRequested,
                profile.OnHpChangeRequested, (name) => { profile.OnDisableProfileRequested(); _listeners.Remove(name); ResetFormationData(); });

            helperObj.transform.rotation = Quaternion.identity;
            ICommandListener listener = helperObj.GetComponent<ICommandListener>();
            _listeners.Add(helperNames[i], listener);

        }

        ResetFormationData();
    }

    public void BuyWeaponToListener(CharacterPlant.Name name, BaseWeapon weapon)
    {
        if (_listeners.ContainsKey(name) == false) return;

        _listeners[name].ReceiveWeapon(weapon);
    }

    public void HealListeners(float hp)
    {
        foreach (var listener in _listeners)
        {
            listener.Value.Heal(hp);
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
