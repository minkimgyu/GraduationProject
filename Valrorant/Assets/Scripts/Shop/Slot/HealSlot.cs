using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealSlot : ItemSlot
{
    float _hpRatio;

    public override void Initialize(HealSlotData data, Func<ShopBlackboard> ReturnBlackboard)
    {
        ResetSlot(data.name.ToString(), data.cost, data.info, data.iconName);

        _hpRatio = data.hpRatio;
        this.ReturnBlackboard = ReturnBlackboard;
        _button.onClick.AddListener(Buy);
    }

    protected override void Buy()
    {
        ShopBlackboard blackboard = ReturnBlackboard();
        blackboard.OnBuyHealpack(_hpRatio);
    }
}
