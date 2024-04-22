using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Pistol, Shotgun�� ����
// LMG, AR, AK�� ���� �ӵ� ����, �ݵ� ���� --> Action, Recoil ��ȭ
// SMG�� �߻� ��� ����, ���� �ӵ� ����, �ݵ� ���� ---> Action, Recoil, Result ��ȭ
// Sniper�� ź ���� ��ȭ, ���� �ӵ� ��ȭ ---> Action, Recoil ��ȭ

abstract public class VariationGun : Gun
{
    public enum Conditon
    {
        Both, // ��� ���ǿ��� ����
        ZoomIn, // �� ���� �ÿ��� ����
        ZoomOut // �� ���� �ÿ��� ����
    }

    protected Dictionary<Tuple<EventType, Conditon>, EventStrategy> _eventStorage = new();
    protected Dictionary<Tuple<EventType, Conditon>, ActionStrategy> _actionStorage = new();
    protected Dictionary<Tuple<EventType, Conditon>, BaseRecoilStrategy> _recoilStorage = new();

    protected void OnZoomRequested(bool nowZoom)
    {
        if (nowZoom) OnZoomIn();
        else OnZoomOut();
    }

    void ResetStrategy(EventStrategy strategy)
    {
        _eventStrategies[EventType.Main].UnlinkEvent(_weaponEventBlackboard);
        strategy.LinkEvent(_weaponEventBlackboard);
        _eventStrategies[EventType.Main] = strategy;
    }

    void ResetStrategy(ActionStrategy strategy)
    {
        _actionStrategies[EventType.Main].UnlinkEvent(_weaponEventBlackboard);
        strategy.LinkEvent(_weaponEventBlackboard);
        _actionStrategies[EventType.Main] = strategy;
    }

    void ResetStrategy(BaseRecoilStrategy strategy)
    {
        _recoilStrategies[EventType.Main].UnlinkEvent(_weaponEventBlackboard);
        strategy.LinkEvent(_weaponEventBlackboard);
        _recoilStrategies[EventType.Main] = strategy;
    }

    protected virtual void OnZoomIn() 
    {
        Tuple<EventType, Conditon> zoomInKey = new(EventType.Main, Conditon.ZoomIn);

        if (_eventStorage.ContainsKey(zoomInKey)) ResetStrategy(_eventStorage[zoomInKey]);
        if (_actionStorage.ContainsKey(zoomInKey)) ResetStrategy(_actionStorage[zoomInKey]);
        if (_recoilStorage.ContainsKey(zoomInKey)) ResetStrategy(_recoilStorage[zoomInKey]);
    }

    protected virtual void OnZoomOut() 
    {
        Tuple<EventType, Conditon> zoomOutKey = new(EventType.Main, Conditon.ZoomOut);

        if (_eventStorage.ContainsKey(zoomOutKey)) ResetStrategy(_eventStorage[zoomOutKey]);
        if (_actionStorage.ContainsKey(zoomOutKey)) ResetStrategy(_actionStorage[zoomOutKey]);
        if (_recoilStorage.ContainsKey(zoomOutKey)) ResetStrategy(_recoilStorage[zoomOutKey]);
    }

    public override void MatchStrategy()
    {
        Tuple<EventType, Conditon> mainBothKey = new(EventType.Main, Conditon.Both);
        Tuple<EventType, Conditon> subBothKey = new(EventType.Sub, Conditon.Both);

        Tuple<EventType, Conditon> mainZoomOutKey = new(EventType.Main, Conditon.ZoomOut);
        Tuple<EventType, Conditon> subZoomOutKey = new(EventType.Sub, Conditon.ZoomOut);


        if (_eventStorage.ContainsKey(mainBothKey)) _eventStrategies[EventType.Main] = _eventStorage[mainBothKey];
        else _eventStrategies[EventType.Main] = _eventStorage[mainZoomOutKey];

        if (_eventStorage.ContainsKey(subBothKey)) _eventStrategies[EventType.Sub] = _eventStorage[subBothKey];
        else _eventStrategies[EventType.Sub] = _eventStorage[subZoomOutKey];


        if (_actionStorage.ContainsKey(mainBothKey)) _actionStrategies[EventType.Main] = _actionStorage[mainBothKey];
        else _actionStrategies[EventType.Main] = _actionStorage[mainZoomOutKey];

        if (_actionStorage.ContainsKey(subBothKey)) _actionStrategies[EventType.Sub] = _actionStorage[subBothKey];
        else _actionStrategies[EventType.Sub] = _actionStorage[subZoomOutKey];


        if (_recoilStorage.ContainsKey(mainBothKey)) _recoilStrategies[EventType.Main] = _recoilStorage[mainBothKey];
        else _recoilStrategies[EventType.Main] = _recoilStorage[mainZoomOutKey];

        if (_recoilStorage.ContainsKey(subBothKey)) _recoilStrategies[EventType.Sub] = _recoilStorage[subBothKey];
        else _recoilStrategies[EventType.Sub] = _recoilStorage[subZoomOutKey];
    }
}