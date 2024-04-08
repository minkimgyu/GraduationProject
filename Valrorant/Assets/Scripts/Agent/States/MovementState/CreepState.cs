using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using Agent.Controller;

namespace Agent.States
{
    public class CreepState : State
    {
        PlayerController storedPlayer;

        public CreepState(PlayerController player)
        {
            storedPlayer = player;
        }

        public override void OnStateFixedUpdate()
        {
            storedPlayer.MovementComponent.Move(true);
        }

        public override void OnStateLateUpdate()
        {
            storedPlayer.ViewComponent.ResetCamera();
        }

        public override void OnStateUpdate()
        {
            storedPlayer.MovementComponent.ResetDirection();
            storedPlayer.ViewComponent.ResetView();

            storedPlayer.MovementComponent.RaiseDisplacementEvent(); // �̵� ���� ���� ������ ���̸� �̹�Ʈ�� �Ѱ���
        }

        public override void CheckStateChange()
        {
            if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            {
                storedPlayer.MovementFSM.SetState(PlayerController.MovementState.Stop);
            }

            if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && Input.GetKeyUp(KeyCode.LeftAlt))
            {
                storedPlayer.MovementFSM.SetState(PlayerController.MovementState.Walk);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                storedPlayer.MovementFSM.SetState(PlayerController.MovementState.Jump);
            }
        }
    }
}