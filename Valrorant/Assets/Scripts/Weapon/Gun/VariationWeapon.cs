using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Pistol, Shotgun�� ����
// LMG, AR, AK�� ���� �ӵ� ����, �ݵ� ���� --> Action, Recoil ��ȭ
// SMG�� �߻� ��� ����, ���� �ӵ� ����, �ݵ� ���� ---> Action, Recoil, Result ��ȭ
// Sniper�� ź ���� ��ȭ, ���� �ӵ� ��ȭ ---> Action, Recoil ��ȭ

abstract public class BaseVariationGun : Gun
{
    protected virtual void LinkEventWhenZoom(EventStrategy actionStrategy, ActionStrategy resultStrategy, RecoilStrategy recoilStrategy) { }

    protected virtual void LinkEventWhenZoom(EventStrategy actionStrategy, ActionStrategy resultStrategy) { }

    protected virtual void LinkEventWhenZoom(EventStrategy actionStrategy, RecoilStrategy recoilStrategy) { }

    protected virtual void LinkEventWhenZoom(RecoilStrategy recoilStrategy) { }

    protected void ChangeActionStrategy(EventStrategy actionStrategy)
    {
        if (_mainEventStrategy != null)
        {
            LinkActionEvent(true, false); // ��ũ ������
            _mainEventStrategy.OnChange(); // �ʱ�ȭ �Լ�
        }

        _mainEventStrategy = actionStrategy; // ���� �Ҵ�

        if (_mainEventStrategy != null) LinkActionEvent(true, true); // ��ũ ������
    }

    protected void ChangeResultStrategy(ActionStrategy resultStrategy)
    {
        if (_mainActionStrategy != null) _mainActionStrategy.OnUnLink(_player); // ��ũ ������

        _mainActionStrategy = resultStrategy; // ���� �Ҵ�

        if (_mainActionStrategy != null) _mainActionStrategy.OnLink(_player); // ��ũ ������
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
    protected EventStrategy _storedMainActionWhenZoomIn;
    protected EventStrategy _storedMainActionWhenZoomOut;

    protected ActionStrategy _storedMainResultWhenZoomIn;
    protected ActionStrategy _storedMainResultWhenZoomOut;

    protected RecoilStrategy _storedMainRecoilWhenZoomIn;
    protected RecoilStrategy _storedMainRecoilWhenZoomOut;

    protected override void LinkEventWhenZoom(EventStrategy actionStrategy, ActionStrategy resultStrategy, RecoilStrategy recoilStrategy)
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
        _subActionStrategy.OnLink(player); // subResult
        _subRecoilStrategy.OnLink(player); // subRecoil
    }
}

/// <summary>
/// ź ����, ���� �ӵ��� ��ȭ�� �ִ� ���
/// </summary>
abstract public class ActionAndRecoilVariationGun : BaseVariationGun
{
    protected EventStrategy _storedMainActionWhenZoomIn; // ���̹��� zoom or nozoom �̷� ������ ��������
    protected EventStrategy _storedMainActionWhenZoomOut;

    protected RecoilStrategy _storedMainRecoilWhenZoomIn;
    protected RecoilStrategy _storedMainRecoilWhenZoomOut;

    protected override void LinkEventWhenZoom(EventStrategy actionStrategy, RecoilStrategy recoilStrategy)
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

        _mainActionStrategy.OnLink(player);
        _subActionStrategy.OnLink(player); // subResult

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

        _mainActionStrategy.OnLink(player);
        _subActionStrategy.OnLink(player); // subResult

        _subRecoilStrategy.OnLink(player);
    }
}

abstract public class NoVariationGun : Gun
{
    protected override void LinkEvent(GameObject player)
    {
        LinkActionEvent(true, true); // mainAction Link
        LinkActionEvent(false, true); // subAction Link

        _mainActionStrategy.OnLink(player);
        _subActionStrategy.OnLink(player);

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

        _mainActionStrategy.OnLink(player);
        _subActionStrategy.OnLink(player);

        _mainRecoilStrategy.OnLink(player);
        _subRecoilStrategy.OnLink(player);
    }
}