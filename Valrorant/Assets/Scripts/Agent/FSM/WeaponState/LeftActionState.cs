using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

public class LeftActionState : IState
{
    Player storedPlayer;

    public LeftActionState(Player player)
    {
        storedPlayer = player;
    }

    public void CheckStateChange()
    {
    }

    public void OnStateCollisionEnter(Collision collision)
    {
    }

    public void OnStateEnter()
    {
        storedPlayer.WeaponHolder.NowEquipedWeapon.StoreCurrentBulletCount();


        storedPlayer.WeaponHolder.NowEquipedWeapon.StartMainAction();
    }

    public void OnStateExit()
    {
        storedPlayer.WeaponHolder.NowEquipedWeapon.EndMainAction();
    }

    public void OnStateFixedUpdate()
    {
    }

    public void OnStateLateUpdate()
    {
    }

    public void OnStateUpdate()
    {
        storedPlayer.WeaponHolder.NowEquipedWeapon.ProgressMainAction();

        // 사격 도중 총알이 떨어진 경우, State에 들어왔을 때는 총알이 존재했지만
        // Update 중 총알이 다 떨어진 경우

        if(storedPlayer.WeaponHolder.NowEquipedWeapon.IsMagazineEmpty())
        {
            storedPlayer.WeaponFSM.SetState(Player.WeaponState.Reload);
        }

        if (Input.GetMouseButtonUp(0))
        {
            storedPlayer.WeaponFSM.RevertToPreviousState();
        }
    }
}
