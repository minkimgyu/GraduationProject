using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class EquipState : IState
{
    Player _storedPlayer;
    Timer _timer;

    public EquipState(Player player)
    {
        _storedPlayer = player;
        _timer = new Timer();
    }

    public void CheckStateChange()
    {
        if (_timer.IsFinish) _storedPlayer.WeaponFSM.SetState(Player.WeaponState.Idle);
    }

    public void OnStateCollisionEnter(Collision collision)
    {
    }

    public void OnStateEnter()
    {
        _storedPlayer.WeaponHolder.ChangeWeapon();
        _timer.Start(_storedPlayer.WeaponHolder.NowEquipedWeapon.EquipFinishTime); // µÙ∑π¿Ã
    }

    public void OnStateExit()
    {

    }

    public void OnStateFixedUpdate()
    {
    }

    public void OnStateLateUpdate()
    {
    }

    public void OnStateUpdate()
    {
        _timer.Update();
    }
}
