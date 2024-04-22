using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

abstract public class BaseStrategy
{
    public virtual void OnUpdate() { }

    public virtual void LinkEvent(WeaponEventBlackboard blackboard) { }

    public virtual void UnlinkEvent(WeaponEventBlackboard blackboard) { }
}
