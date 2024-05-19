using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ReviveSlot : ItemSlot
{
    public override void Initialize(ReviveSlotData data, Func<ShopBlackboard> ReturnBlackboard)
    {
        ResetSlot(data.name.ToString(), data.cost, data.info, data.iconName);

        this.ReturnBlackboard = ReturnBlackboard;
        _button.onClick.AddListener(Buy);
    }

    protected override void BuyToHelper(CharacterPlant.Name name)
    {
        if (ReturnBlackboard().CanBuy(_cost) == false) return;

        ReturnBlackboard().Buy(_cost);

        ShopBlackboard blackboard = ReturnBlackboard();
        blackboard.ReviveHelper?.Invoke(name);
    }
}
