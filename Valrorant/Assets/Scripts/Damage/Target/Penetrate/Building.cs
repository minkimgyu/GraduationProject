using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : PenetrateTarget
{
    protected override void Initialize()
    {
        _durability = 10;

        _hitEffect = new Dictionary<IEffectable.ConditionType, IEffectable.EffectName>()
        {
            {IEffectable.ConditionType.Penetration, IEffectable.EffectName.BulletHole },
            {IEffectable.ConditionType.NonPenetration, IEffectable.EffectName.WallFragmentation },
            {IEffectable.ConditionType.Stabbing, IEffectable.EffectName.KnifeMark }
        };
    }
}
