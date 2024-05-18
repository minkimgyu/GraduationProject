using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableSlot : ItemSlot
{
    float _hpPoint;
    float _armorPoint;

    public override void ResetData(float hpPoint, float armorPoint)
    {
        _hpPoint = hpPoint;
        _armorPoint = armorPoint;
    }

    protected override void Buy()
    {
        ShopBlackboard blackboard = ReturnBlackboard();
        blackboard.OnBuyHealpack(_hpPoint, _armorPoint);
    }
}
