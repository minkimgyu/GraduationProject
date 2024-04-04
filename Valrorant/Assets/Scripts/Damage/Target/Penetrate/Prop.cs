using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : PenetrateTarget
{
    protected override void Initialize()
    {
        _durability = 1;

        _hitEffect = new Dictionary<IEffectable.ConditionType, IEffectable.EffectName>()
        {
            {IEffectable.ConditionType.Penetration, IEffectable.EffectName.ObjectFragmentation },
            {IEffectable.ConditionType.NonPenetration, IEffectable.EffectName.ObjectFragmentation },
            {IEffectable.ConditionType.Stabbing, IEffectable.EffectName.ObjectFragmentation }
        };
    }
}
