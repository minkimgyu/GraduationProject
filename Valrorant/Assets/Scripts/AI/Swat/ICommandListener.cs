using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AI;

public interface ICommandListener
{
    void GoToBuildFormationState(FormationData data);
    void GoToFreeRoleState();
}