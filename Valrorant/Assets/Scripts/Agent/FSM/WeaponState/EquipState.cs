using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class EquipState : IState
{
    Player _storedPlayer;

    Timer timer = new Timer();

    public EquipState(Player player)
    {
        _storedPlayer = player;
    }

    public void CheckStateChange()
    {
    }

    public void OnStateCollisionEnter(Collision collision)
    {
    }

    public void OnStateEnter()
    {
        _storedPlayer.WeaponHolder.ChangeWeapon();
        timer.Start(_storedPlayer.WeaponHolder.NowEquipedWeapon.EquipFinishTime); // µÙ∑π¿Ã
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
        timer.Update();
        if (timer.IsTimerFinish())
        {
            _storedPlayer.WeaponFSM.SetState(Player.WeaponState.Idle);
        }
    }
}
