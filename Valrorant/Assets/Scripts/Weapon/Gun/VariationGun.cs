using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� �����ϰų� ������ �� ������Ʈ ���Ͽ� ��ȭ�� �ִ� Ŭ����
/// </summary>
abstract public class AllVariationGun : Gun
{
    protected ActionStrategy _storedMainActionStrategyWhenZoomIn;
    protected ActionStrategy _storedMainActionStrategyWhenZoomOut;

    protected ResultStrategy _storedMainResultStrategyWhenZoomIn;
    protected ResultStrategy _storedMainResultStrategyWhenZoomOut;

    protected RecoilStrategy _storedMainRecoilStrategyWhenZoomIn;
    protected RecoilStrategy _storedMainRecoilStrategyWhenZoomOut;

    void LinkEventWhenZoom(ActionStrategy actionStrategy, ResultStrategy resultStrategy, RecoilStrategy recoilStrategy)
    {
        if (_mainActionStrategy != null && _mainResultStrategy != null && _mainRecoilStrategy != null)  // ���⼭ �̺�Ʈ�� �������ְ�
        {
            LinkActionEvent(true, false);
            _mainResultStrategy.OnUnLink(_player);
            _mainRecoilStrategy.OnUnlink(_player);

            _mainActionStrategy.OnChange(); // �ʱ�ȭ �Լ�
            _mainRecoilStrategy.ResetRecoil(); // ���⼭ ���� ������ �ʱ�ȭ
        }

        _mainActionStrategy = actionStrategy; // ���� �Ҵ�
        _mainResultStrategy = resultStrategy;
        _mainRecoilStrategy = recoilStrategy;

        // ���� �ٽ� �̺�Ʈ �����Ű��
        if (_mainActionStrategy != null && _mainResultStrategy != null && _mainRecoilStrategy != null)
        {
            LinkActionEvent(true, true);
            _mainResultStrategy.OnLink(_player);
            _mainRecoilStrategy.OnLink(_player);

            _mainRecoilStrategy.RecoverRecoil(); // ���⼭ �ݵ�ȸ�� �־���
        }
    }

    protected override void OnZoomIn() 
    {
        LinkEventWhenZoom(_storedMainActionStrategyWhenZoomIn, _storedMainResultStrategyWhenZoomIn, _storedMainRecoilStrategyWhenZoomIn);
    }

    protected override void OnZoomOut() 
    {
        LinkEventWhenZoom(_storedMainActionStrategyWhenZoomOut, _storedMainResultStrategyWhenZoomOut, _storedMainRecoilStrategyWhenZoomOut);
    }

    protected override void LinkEvent(GameObject player)
    {
        OnZoomOut(); // �ʱ� �Ҵ翡�� OnZoomOut ����

        LinkActionEvent(false, true); // subAction Link
        _subResultStrategy.OnLink(player); // subResult
        _subRecoilStrategy.OnLink(player); // subRecoil
        _reloadStrategy.OnLink();
    }
}

/// <summary>
/// ź ����, ���� �ӵ��� ��ȭ�� �ִ� ���
/// </summary>
abstract public class SpreadAndSpeedVariationGun : Gun
{
    protected ActionStrategy _storedMainActionWhenZoomIn; // ���̹��� zoom or nozoom �̷� ������ ��������
    protected ActionStrategy _storedMainActionWhenZoomOut;

    protected ResultStrategy _storedMainResultWhenZoomIn;
    protected ResultStrategy _storedMainResultWhenZoomOut;

    void LinkEventWhenZoom(ActionStrategy actionStrategy, ResultStrategy resultStrategy)
    {
        if (_mainActionStrategy != null && _mainResultStrategy != null)  // ���⼭ �̺�Ʈ�� �������ְ�
        {
            LinkActionEvent(true, false);
            _mainResultStrategy.OnUnLink(_player);

            _mainActionStrategy.OnChange(); // �ʱ�ȭ �Լ�
        }

        // ���Ҵ�
        _mainActionStrategy = actionStrategy; // �ʱ�ȭ �Լ��� �۵��Ǿ��� + �߰��� �̺�Ʈ ���� ���� �������� ����
        _mainResultStrategy = resultStrategy;

        // ���� �ٽ� �̺�Ʈ �����Ű��

        if (_mainActionStrategy != null && _mainResultStrategy != null)
        {
            LinkActionEvent(true, true);
            _mainResultStrategy.OnLink(_player);
        }
    }

    protected override void OnZoomIn() 
    {
        LinkEventWhenZoom(_storedMainActionWhenZoomIn, _storedMainResultWhenZoomIn);
    }

    protected override void OnZoomOut() // --> Initialize������ �̰� ���
    {
        LinkEventWhenZoom(_storedMainActionWhenZoomOut, _storedMainResultWhenZoomOut);
    }

    protected override void LinkEvent(GameObject player)
    {
        OnZoomOut(); // �ʱ� �Ҵ翡�� OnZoomOut ����

        LinkActionEvent(false, true); // subAction
        _subResultStrategy.OnLink(player); // subResult

        _mainRecoilStrategy.OnLink(player);
        _subRecoilStrategy.OnLink(player);
        _reloadStrategy.OnLink();
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
        _reloadStrategy.OnLink();
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
        _reloadStrategy.OnLink();
    }
}

//if (nowZoom)
//{
//    //_mainActionStrategy.OnChange();

//    //_mainActionStrategy = _burstAttackAction; // �⺻�� AutoAction
//    //_mainRecoilStrategy = _burstRecoilGenerator; // �⺻�� AutoAction

//    OnZoomIn();
//}
//else
//{
//    //_burstAttackAction.OnChange();

//    //_mainActionStrategy = _autoAttackAction; // �⺻�� AutoAction
//    //_mainRecoilStrategy = _autoRecoilGenerator; // �⺻�� AutoAction

//    OnZoomOut();
//}