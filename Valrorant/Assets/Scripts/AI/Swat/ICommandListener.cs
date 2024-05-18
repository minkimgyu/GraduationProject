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

    Vector3 ReturnPos(); // 위치 반환
}