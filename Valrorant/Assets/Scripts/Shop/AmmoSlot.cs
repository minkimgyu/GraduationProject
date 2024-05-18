using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoSlot : ItemSlot
{
    protected override void Buy()
    {
        ShopBlackboard blackboard = ReturnBlackboard();
        blackboard.OnBuyAmmo();
    }
}
