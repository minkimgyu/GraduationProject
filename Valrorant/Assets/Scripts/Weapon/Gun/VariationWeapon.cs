using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Pistol, Shotgun은 없음
// LMG, AR, AK는 연사 속도 변경, 반동 변경 --> Action, Recoil 변화
// SMG는 발사 방식 변경, 연사 속도 변경, 반동 변경 ---> Action, Recoil, Result 변화
// Sniper는 탄 퍼짐 변화, 공격 속도 변화 ---> Action, Recoil 변화

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
            LinkActionEvent(true, false); // 링크 끊어줌
            _mainEventStrategy.OnChange(); // 초기화 함수
        }

        _mainEventStrategy = actionStrategy; // 새로 할당

        if (_mainEventStrategy != null) LinkActionEvent(true, true); // 링크 끊어줌
    }

    protected void ChangeResultStrategy(ActionStrategy resultStrategy)
    {
        if (_mainActionStrategy != null) _mainActionStrategy.OnUnLink(_player); // 링크 끊어줌

        _mainActionStrategy = resultStrategy; // 새로 할당

        if (_mainActionStrategy != null) _mainActionStrategy.OnLink(_player); // 링크 끊어줌
    }

    protected void ChangeRecoilStrategy(RecoilStrategy recoilStrategy)
    {
        if (_mainRecoilStrategy != null)
        {
            _mainRecoilStrategy.OnUnlink(_player); // 링크 끊어줌
            _mainRecoilStrategy.ResetValues(); // 여기서 기존 리코일 초기화
        }

        _mainRecoilStrategy = recoilStrategy; // 새로 할당

        if (_mainRecoilStrategy != null)
        {
            _mainRecoilStrategy.OnLink(_player); // 링크 끊어줌
            _mainRecoilStrategy.RecoverRecoil(); // 여기서 반동회복 넣어줌
        }
    }
}

/// <summary>
/// 줌을 적용하거나 해제할 때 스테이트 패턴에 변화가 있는 클레스
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
        OnZoomOut(); // 초기 할당에는 OnZoomOut 적용

        LinkActionEvent(false, true); // subAction Link
        _subActionStrategy.OnLink(player); // subResult
        _subRecoilStrategy.OnLink(player); // subRecoil
    }
}

/// <summary>
/// 탄 퍼짐, 연사 속도의 변화가 있는 경우
/// </summary>
abstract public class ActionAndRecoilVariationGun : BaseVariationGun
{
    protected EventStrategy _storedMainActionWhenZoomIn; // 네이밍을 zoom or nozoom 이런 식으로 가져가자
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

    protected override void OnZoomOut() // --> Initialize에서는 이거 사용
    {
        LinkEventWhenZoom(_storedMainActionWhenZoomOut, _storedMainRecoilWhenZoomOut);
    }

    protected override void LinkEvent(GameObject player)
    {
        OnZoomOut(); // 초기 할당에는 OnZoomOut 적용

        LinkActionEvent(false, true); // subAction

        _mainActionStrategy.OnLink(player);
        _subActionStrategy.OnLink(player); // subResult

        _subRecoilStrategy.OnLink(player);
    }
}

/// <summary>
/// 탄 퍼짐, 연사 속도의 변화가 있는 경우
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

    protected override void OnZoomOut() // --> Initialize에서는 이거 사용
    {
        LinkEventWhenZoom(_storedMainRecoilWhenZoomOut);
    }

    protected override void LinkEvent(GameObject player)
    {
        OnZoomOut(); // 초기 할당에는 OnZoomOut 적용

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