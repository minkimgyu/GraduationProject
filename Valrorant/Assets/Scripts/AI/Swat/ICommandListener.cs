using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AI;

public interface ICommandListener
{
    void GoToBuildFormationState();
    void GoToFreeRoleState();

    void ResetFormationData(FormationData data);
    void ReceiveWeapon(BaseWeapon weapon);
    void Heal(float hpRatio);
    void RefillAmmo();

    Vector3 ReturnPos(); // 위치 반환
    void ResetPos(Vector3 pos); // 위치 반환
}