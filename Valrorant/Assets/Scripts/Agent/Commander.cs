using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class Commander : MonoBehaviour
{
    [SerializeField] List<GameObject> _listenerGO;
    List<ICommandListener> _listeners = new List<ICommandListener>();

    public void Initialize()
    {
        for (int i = 0; i < _listenerGO.Count; i++)
        {
            ICommandListener listener = _listenerGO[i].GetComponent<ICommandListener>();
            _listeners.Add(listener);
        }
    }

    public void BuildFormation()
    {
        for (int i = 0; i < _listeners.Count; i++)
        {
            FormationData data = new FormationData(i + 1 , _listeners.Count);
            _listeners[i].GoToBuildFormationState(data);
        }
    }

    public void FreeRole()
    {
        for (int i = 0; i < _listeners.Count; i++)
        {
            _listeners[i].GoToFreeRoleState();
        }
    }

    public void PickUpWeapon()
    {

    }

    public void SetPriorityTarget()
    {

    }
}
