using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Pistol, Shotgun�� ����
// LMG, AR, AK�� ���� �ӵ� ����, �ݵ� ���� --> Action, Recoil ��ȭ
// SMG�� �߻� ��� ����, ���� �ӵ� ����, �ݵ� ���� ---> Action, Recoil, Result ��ȭ
// Sniper�� ź ���� ��ȭ, ���� �ӵ� ��ȭ ---> Action, Recoil ��ȭ

abstract public class BaseVariationGun : Gun
{
    protected virtual void LinkEventWhenZoom(ActionStrategy actionStrategy, ResultStrategy resultStrategy, RecoilStrategy recoilStrategy) { }

    protected virtual void LinkEventWhenZoom(ActionStrategy actionStrategy, ResultStrategy resultStrategy) { }

    protected virtual void LinkEventWhenZoom(ActionStrategy actionStrategy, RecoilStrategy recoilStrategy) { }

    protected virtual void LinkEventWhenZoom(RecoilStrategy recoilStrategy) { }

    protected void ChangeActionStrategy(ActionStrategy actionStrategy)
    {
        if (_mainActionStrategy != null)
        {
            LinkActionEvent(true, false); // ��ũ ������
            _mainActionStrategy.OnChange(); // �ʱ�ȭ �Լ�
        }

        _mainActionStrategy = actionStrategy; // ���� �Ҵ�

        if (_mainActionStrategy != null) LinkActionEvent(true, true); // ��ũ ������
    }

    protected void ChangeResultStrategy(ResultStrategy resultStrategy)
    {
        if (_mainResultStrategy != null) _mainResultStrategy.OnUnLink(_player); // ��ũ ������

        _mainResultStrategy = resultStrategy; // ���� �Ҵ�

        if (_mainResultStrategy != null) _mainResultStrategy.OnLink(_player); // ��ũ ������
    }

    protected void ChangeRecoilStrategy(RecoilStrategy recoilStrategy)
    {
        if (_mainRecoilStrategy != null)
        {
            _mainRecoilStrategy.OnUnlink(_player); // ��ũ ������
            _mainRecoilStrategy.ResetValues(); // ���⼭ ���� ������ �ʱ�ȭ
        }

        _mainRecoilStrategy = recoilStrategy; // ���� �Ҵ�

        if (_mainRecoilStrategy != null)
        {
            _mainRecoilStrategy.OnLink(_player); // ��ũ ������
            _mainRecoilStrategy.RecoverRecoil(); // ���⼭ �ݵ�ȸ�� �־���
        }
    }
}

/// <summary>
/// ���� �����ϰų� ������ �� ������Ʈ ���Ͽ� ��ȭ�� �ִ� Ŭ����
/// </summary>
abstract public class AllVariationGun : BaseVariationGun
{
    protected ActionStrategy _storedMainActionWhenZoomIn;
    protected ActionStrategy _storedMainActionWhenZoomOut;

    protected ResultStrategy _storedMainResultWhenZoomIn;
    protected ResultStrategy _storedMainResultWhenZoomOut;

    protected RecoilStrategy _storedMainRecoilWhenZoomIn;
    protected RecoilStrategy _storedMainRecoilWhenZoomOut;

    protected override void LinkEventWhenZoom(ActionStrategy actionStrategy, ResultStrategy resultStrategy, RecoilStrategy recoilStrategy)
    {
        ChangeActionStrategy(actionStrategy);
        ChangeResultStrategy(resultStrategy);
        ChangeRecoilStrategy(recoilStrategy);
    }

    protected override void OnZoomIn() 
    {
        LinkEventWhenZoom(_storedMainActionWhenZoomIn, _storedMainResultWhenZoomIn, _storedMainRecoilWhenZoomIn);
    }

    protected override void OnZoomOut() 
    {
        LinkEventWhenZoom(_storedMainActionWhenZoomOut, _storedMainResultWhenZoomOut, _storedMainRecoilWhenZoomOut);
    }

    protected override void LinkEvent(GameObject player)
    {
        OnZoomOut(); // �ʱ� �Ҵ翡�� OnZoomOut ����

        LinkActionEvent(false, true); // subAction Link
        _subResultStrategy.OnLink(player); // subResult
        _subRecoilStrategy.OnLink(player); // subRecoil
    }
}

/// <summary>
/// ź ����, ���� �ӵ��� ��ȭ�� �ִ� ���
/// </summary>
abstract public class ActionAndRecoilVariationGun : BaseVariationGun
{
    protected ActionStrategy _storedMainActionWhenZoomIn; // ���̹��� zoom or nozoom �̷� ������ ��������
    protected ActionStrategy _storedMainActionWhenZoomOut;

    protected RecoilStrategy _storedMainRecoilWhenZoomIn;
    protected RecoilStrategy _storedMainRecoilWhenZoomOut;

    protected override void LinkEventWhenZoom(ActionStrategy actionStrategy, RecoilStrategy recoilStrategy)
    {
        ChangeActionStrategy(actionStrategy);
        ChangeRecoilStrategy(recoilStrategy);
    }

    protected override void OnZoomIn() 
    {
        LinkEventWhenZoom(_storedMainActionWhenZoomIn, _storedMainRecoilWhenZoomIn);
    }

    protected override void OnZoomOut() // --> Initialize������ �̰� ���
    {
        LinkEventWhenZoom(_storedMainActionWhenZoomOut, _storedMainRecoilWhenZoomOut);
    }

    protected override void LinkEvent(GameObject player)
    {
        OnZoomOut(); // �ʱ� �Ҵ翡�� OnZoomOut ����

        LinkActionEvent(false, true); // subAction

        _mainResultStrategy.OnLink(player);
        _subResultStrategy.OnLink(player); // subResult

        _subRecoilStrategy.OnLink(player);
    }
}

/// <summary>
/// ź ����, ���� �ӵ��� ��ȭ�� �ִ� ���
/// </summary>
abstract public class RecoilVariationGun : BaseVariationGun
{
    protected RecoilStrategy _storedMainRecoilWhenZoomIn;
    protected RecoilStrategy _storedMainRecoilWhenZoomOut;

    protected override void LinkEventWhenZoom(RecoilStrategy recoilStrategy)
    {
        ChangeRecoilStrategy(recoilStrategy);
    }

    protected override void OnZoomIn()
    {
        LinkEventWhenZoom(_storedMainRecoilWhenZoomIn);
    }

    protected override void OnZoomOut() // --> Initialize������ �̰� ���
    {
        LinkEventWhenZoom(_storedMainRecoilWhenZoomOut);
    }

    protected override void LinkEvent(GameObject player)
    {
        OnZoomOut(); // �ʱ� �Ҵ翡�� OnZoomOut ����

        LinkActionEvent(true, true); // mainAction Link
        LinkActionEvent(false, true); // subAction

        _mainResultStrategy.OnLink(player);
        _subResultStrategy.OnLink(player); // subResult

        _subRecoilStrategy.OnLink(player);
    }
}

abstract public class NoVariationGun : Gun
{
    protected override void LinkEvent(GameObject player)
    {
        LinkActionEvent(true, true); // mainAction Link
        LinkActionEvent(false, true); // subAction Link

        _mainResultStrategy.OnLink(player);
        _subResultStrategy.OnLink(player);

        _mainRecoilStrategy.OnLink(player);
        _subRecoilStrategy.OnLink(player);
    }
}

abstract public class NoVariationWeapon : BaseWeapon
{
    protected override void LinkEvent(GameObject player)
    {
        LinkActionEvent(true, true); // mainAction Link
        LinkActionEvent(false, true); // subAction Link

        _mainResultStrategy.OnLink(player);
        _subResultStrategy.OnLink(player);

        _mainRecoilStrategy.OnLink(player);
        _subRecoilStrategy.OnLink(player);
    }
}