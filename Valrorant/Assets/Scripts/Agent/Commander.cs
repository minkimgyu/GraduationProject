using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class Commander : MonoBehaviour
{
    Dictionary<CharacterPlant.Name, ICommandListener> _listeners = new Dictionary<CharacterPlant.Name, ICommandListener>();
    CharacterPlant.Name[] helperNames = { CharacterPlant.Name.Oryx, CharacterPlant.Name.Rook, CharacterPlant.Name.Warden };

    float _startRange;

    public void Initialize(float startRange)
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
            Transform helper = plant.Create(helperNames[i], player.ReturnPos, profile.OnWeaponProfileChangeRequested, 
                profile.OnHpChangeRequested, () => { profile.OnDisableProfileRequested(); _listeners.Remove(helperNames[i]); ResetFormationData(); });

            helper.position = new Vector3(transform.position.x + startPos.y, transform.position.y, transform.position.z + startPos.x);
            helper.rotation = Quaternion.identity;

            ICommandListener listener = helper.GetComponent<ICommandListener>();
            _listeners.Add(helperNames[i], listener);

        }

        ResetFormationData();
    }

    public void AddListener()
    {
        bool canAdd = false;
        CharacterPlant.Name name = CharacterPlant.Name.Oryx;

        for (int i = 0; i < helperNames.Length; i++)
        {
            bool containKey = _listeners.ContainsKey(helperNames[i]);
            if (containKey == true) continue;

            name = helperNames[i];
            canAdd = true;
            break;
        }

        if (canAdd == false) return;

        CharacterPlant plant = FindObjectOfType<CharacterPlant>();
        HelperViewer viewer = FindObjectOfType<HelperViewer>();
        Player player = FindObjectOfType<Player>();
        ProfileViewer profile = viewer.ReturnProfile(name);

        profile.OnActiveProfileRequested(); // 재활성화

        Vector2 startPos = Random.insideUnitCircle * _startRange;
        Transform helper = plant.Create(name, player.ReturnPos, profile.OnWeaponProfileChangeRequested,
            profile.OnHpChangeRequested, () => { profile.OnDisableProfileRequested(); _listeners.Remove(name); ResetFormationData(); });

        helper.position = new Vector3(transform.position.x + startPos.y, transform.position.y, transform.position.z + startPos.x);
        helper.rotation = Quaternion.identity;

        ICommandListener listener = helper.GetComponent<ICommandListener>();
        _listeners.Add(name, listener);

        ResetFormationData();
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
