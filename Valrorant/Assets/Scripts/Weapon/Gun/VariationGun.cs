using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 줌을 적용하거나 해제할 때 스테이트 패턴에 변화가 있는 클레스
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
        if (_mainActionStrategy != null && _mainResultStrategy != null && _mainRecoilStrategy != null)  // 여기서 이벤트를 해제해주고
        {
            LinkActionEvent(true, false);
            _mainResultStrategy.OnUnLink(_player);
            _mainRecoilStrategy.OnUnlink(_player);

            _mainActionStrategy.OnChange(); // 초기화 함수
            _mainRecoilStrategy.ResetRecoil(); // 여기서 기존 리코일 초기화
        }

        _mainActionStrategy = actionStrategy; // 새로 할당
        _mainResultStrategy = resultStrategy;
        _mainRecoilStrategy = recoilStrategy;

        // 이후 다시 이벤트 연결시키기
        if (_mainActionStrategy != null && _mainResultStrategy != null && _mainRecoilStrategy != null)
        {
            LinkActionEvent(true, true);
            _mainResultStrategy.OnLink(_player);
            _mainRecoilStrategy.OnLink(_player);

            _mainRecoilStrategy.RecoverRecoil(); // 여기서 반동회복 넣어줌
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
        OnZoomOut(); // 초기 할당에는 OnZoomOut 적용

        LinkActionEvent(false, true); // subAction Link
        _subResultStrategy.OnLink(player); // subResult
        _subRecoilStrategy.OnLink(player); // subRecoil
        _reloadStrategy.OnLink();
    }
}

/// <summary>
/// 탄 퍼짐, 연사 속도의 변화가 있는 경우
/// </summary>
abstract public class SpreadAndSpeedVariationGun : Gun
{
    protected ActionStrategy _storedMainActionWhenZoomIn; // 네이밍을 zoom or nozoom 이런 식으로 가져가자
    protected ActionStrategy _storedMainActionWhenZoomOut;

    protected ResultStrategy _storedMainResultWhenZoomIn;
    protected ResultStrategy _storedMainResultWhenZoomOut;

    void LinkEventWhenZoom(ActionStrategy actionStrategy, ResultStrategy resultStrategy)
    {
        if (_mainActionStrategy != null && _mainResultStrategy != null)  // 여기서 이벤트를 해제해주고
        {
            LinkActionEvent(true, false);
            _mainResultStrategy.OnUnLink(_player);

            _mainActionStrategy.OnChange(); // 초기화 함수
        }

        // 재할당
        _mainActionStrategy = actionStrategy; // 초기화 함수가 작동되야함 + 추가로 이벤트 연결 해제 시퀀스도 넣자
        _mainResultStrategy = resultStrategy;

        // 이후 다시 이벤트 연결시키기

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

    protected override void OnZoomOut() // --> Initialize에서는 이거 사용
    {
        LinkEventWhenZoom(_storedMainActionWhenZoomOut, _storedMainResultWhenZoomOut);
    }

    protected override void LinkEvent(GameObject player)
    {
        OnZoomOut(); // 초기 할당에는 OnZoomOut 적용

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

//    //_mainActionStrategy = _burstAttackAction; // 기본은 AutoAction
//    //_mainRecoilStrategy = _burstRecoilGenerator; // 기본은 AutoAction

//    OnZoomIn();
//}
//else
//{
//    //_burstAttackAction.OnChange();

//    //_mainActionStrategy = _autoAttackAction; // 기본은 AutoAction
//    //_mainRecoilStrategy = _autoRecoilGenerator; // 기본은 AutoAction

//    OnZoomOut();
//}