using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageUtility;

[System.Serializable]
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

    bool stopMainAction = false;
    bool stopSubAction = false;

    Coroutine stopMainActionRoutine;
    Coroutine stopSubActionRoutine;

    [SerializeField]
    DirectionData _mainAttackDamageData;

    [SerializeField]
    DirectionData _subAttackDamageData;

    public override void OnEquip()
    {
        base.OnEquip();
        OnActiveContainerRequested?.Invoke(false);
    }

    protected override void ChainMainActionProgressEvent()
    {
        if (stopMainAction) return;


        if (attackTime == 0)
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
        StartCoroutine(DoSimpleAttackRoutine(mainAttackEffectDelayTime[actionIndex - 1]));

        stopSubAction = true;

        // 돌아가는 중이라면 취소시키고 다시 돌리기
        if(stopSubActionRoutine != null) StopCoroutine(stopSubActionRoutine);
        stopSubActionRoutine = StartCoroutine(DelaySubActionRoutine(_subAttackDelay));
    }

    protected override void ChainSubActionProgressEvent()
    {
        if (stopSubAction) return;


        _ownerAnimator.Play(_weaponName + "SubAction", -1, 0);
        StartCoroutine(DoHardAttackRoutine(subAttackEffectDelayTime));

        stopMainAction = true;

        // 돌아가는 중이라면 취소시키고 다시 돌리기
        if (stopMainActionRoutine != null) StopCoroutine(stopMainActionRoutine);
        stopMainActionRoutine = StartCoroutine(DelayMainActionRoutine(_mainAttackDelay));
    }

    IEnumerator DoSimpleAttackRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        _mainResult.Do();
    }

    IEnumerator DoHardAttackRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        _subResult.Do();
    }

    IEnumerator DelayMainActionRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        stopMainAction = false;
        stopMainActionRoutine = null;
    }

    IEnumerator DelaySubActionRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        stopSubAction = false;
        stopSubActionRoutine = null;
    }

    public override void Initialize(GameObject player, Transform cam, Animator ownerAnimator)
    {
        base.Initialize(player, cam, ownerAnimator);

        _mainResult = new KnifeAttack(_camTransform, _range, _hitEffectName, _targetLayer, _mainAttackDamageData);
        _subResult = new KnifeAttack(_camTransform, _range, _hitEffectName, _targetLayer, _subAttackDamageData);

        _mainAction = new AutoAttackAction(_mainAttackDelay);
        _subAction = new AutoAttackAction(_subAttackDelay);

        _mainRecoilGenerator = new NoRecoilGenerator();
        _subRecoilGenerator = new NoRecoilGenerator();

        LinkActionStrategy();
    }
}
