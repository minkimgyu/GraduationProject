using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : BaseWeapon
{
    [SerializeField]
    float _mainAttackDelay;

    [SerializeField]
    float _subAttackDelay;

    const int actionCount = 3;

    [SerializeField]
    float attackLinkTime = 2.8f;

    [SerializeField]
    float[] mainAttackEffectDelayTime = new float[actionCount];

    [SerializeField]
    float subAttackEffectDelayTime;

    float attackTime = 0;
    int actionIndex = 1;

    protected override void ChainMainAction()
    {
        if(attackTime == 0)
        {
            actionIndex = 1;
            attackTime = Time.time;
        }
        else
        {
            if(Time.time - attackTime < attackLinkTime)
            {
                // 연계 성공
                actionIndex += 1;
                attackTime = Time.time;
                if (actionIndex > actionCount) actionIndex = 1;
            }
            else
            {
                // 연계 실패
                attackTime = 0;
                actionIndex = 1;
            }
        }

        _ownerAnimator.Play(_weaponName + "MainAction" + actionIndex, -1, 0);
        Invoke("DoSimpleAttack", mainAttackEffectDelayTime[actionIndex - 1]); //0.16f
    }

    protected override void ChainSubAction()
    {
        _ownerAnimator.Play(_weaponName + "SubAction", -1, 0);
        Invoke("DoHardAttack", subAttackEffectDelayTime);
    }

    void DoSimpleAttack()
    {
        _mainResult.Attack();
    }

    void DoHardAttack()
    {
        _subResult.Attack();
    }

    protected override void Awake()
    {
        base.Awake();
        _mainResult = new KnifeAttack(_camTransform, _range, _hitEffectName, _targetLayer);
        _subResult = new KnifeAttack(_camTransform, _range, _hitEffectName, _targetLayer);

        _mainAction = new AutoAttackAction(_mainAttackDelay);
        _subAction = new AutoAttackAction(_subAttackDelay);

        _mainAction.OnActionStart = ChainMainAction;
        _subAction.OnActionStart = ChainSubAction;
    }
}
