using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Events/AimEvent", fileName = "AimEvent")]
public class AimEvent : ScriptableObject
{
    public Action<GameObject, bool, float, float> OnAimRequested;
    public Action<GameObject, bool> OnAimOffInstantlyRequested;

    public void RaiseEvent(GameObject scope, bool isAiming)
    {
        if (OnAimOffInstantlyRequested != null)
        {
            OnAimOffInstantlyRequested(scope, isAiming);
        }
        else
        {
            Debug.Log("연결된 이밴트가 없습니다.");
        }
    }

    public void RaiseEvent(GameObject scope, bool isAiming, float zoomDuration, float scopeOnDelay)
    {
        if(OnAimRequested != null)
        {
            OnAimRequested(scope, isAiming, zoomDuration, scopeOnDelay);
        }
        else
        {
            Debug.Log("연결된 이밴트가 없습니다.");
        }
    }
}
