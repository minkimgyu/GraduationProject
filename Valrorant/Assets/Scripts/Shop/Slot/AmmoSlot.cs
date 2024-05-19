using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AmmoSlot : ItemSlot
{
    public override void Initialize(AmmoSlotData data, Func<ShopBlackboard> ReturnBlackboard)
    {
        ResetSlot(data.name.ToString(), data.cost, data.info, data.iconName);

        this.ReturnBlackboard = ReturnBlackboard;
        _button.onClick.AddListener(Buy);
    }

    protected override void Buy()
    {
        ShopBlackboard blackboard = ReturnBlackboard();
        blackboard.OnBuyAmmo();
    }
}
