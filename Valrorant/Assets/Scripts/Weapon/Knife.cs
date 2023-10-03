using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : BaseWeapon
{
    [SerializeField]
    float _mainAttackDelay;

    [SerializeField]
    float _subAttackDelay;

    AttackStrategy simpleAttack;
    AttackStrategy hardAttack;

    protected override void ChainMainAction()
    {
        simpleAttack.Attack();
        base.ChainMainAction();
    }

    protected override void ChainSubAction()
    {
        hardAttack.Attack();
        base.ChainSubAction();
    }

    void Start()
    {
        simpleAttack = new KnifeAttack(_camTransform, _range, _hitEffectName, _targetLayer);
        hardAttack = new KnifeAttack(_camTransform, _range, _hitEffectName, _targetLayer);

        _mainAction = new AutoAttackAction(_mainAttackDelay);
        _subAction = new AutoAttackAction(_subAttackDelay);

        _mainAction.OnActionStart = ChainMainAction;
        _subAction.OnActionStart = ChainSubAction;
    }
}
