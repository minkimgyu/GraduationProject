using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Events/AimEvent", fileName = "AimEvent")]
public class AimEvent : ScriptableObject
{
    public Action<bool> OnAimRequested;

    public void RaiseEvent(bool isAiming)
    {
        if(OnAimRequested != null)
        {
            OnAimRequested(isAiming);
        }
        else
        {
            Debug.Log("연결된 이밴트가 없습니다.");
        }
    }
}
