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
        BulletHole, // �� ���� �� �Ѿ� �ڱ�
        WallFragmentation, // �� ���� ���� �� �Ѿ� ����ȭ
        KnifeMark, // Į�� �ڱ��� �� ���

        ObjectFragmentation // �����Ͽ� ������Ʈ�� �μ����� ���
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