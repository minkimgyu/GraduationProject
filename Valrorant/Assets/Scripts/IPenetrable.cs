using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPenetrable : IAttachedObject
{
    public float ReturnDurability();
}

public interface IAttachedObject
{
    public GameObject ReturnAttachedObject();
}

public interface IEffectable
{
    public enum EffectName
    {
        BulletHole, // 벽 관통 시 총알 자국
        WallFragmentation, // 벽 관통 실패 시 총알 파편화
        KnifeMark, // 칼로 자국이 난 경우

        ObjectFragmentation // 관통하여 오브젝트가 부서지는 경우
    }

    public enum ConditionType
    {
        Penetration,
        NonPenetration,
        Stabbing
    }

    public bool CanReturnHitEffectName(ConditionType effectType);

    public string ReturnHitEffectName(ConditionType effectType);
}