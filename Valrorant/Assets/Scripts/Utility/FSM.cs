using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

namespace FSM
{
    abstract public class State : BaseState
    {
        public override void CheckStateChange() { }

        public override void OnMessageReceived(string message, BaseWeapon.Type weaponType) { } // 여러 개 만들어서 상속
        public override void OnMessageReceived(string message, bool containSameType) { }
        public override void OnMessageReceived(string message, BaseWeapon newWeapon) { }
        public override void OnMessageReceived(string message, float multiplier) { }

        public override void OnStateFixedUpdate() { }
        public override void OnStateLateUpdate() { }
        public override void OnNoiseReceived() { }
        public override void OnDamaged(float damage) { }
        public override void OnHeal(float hpPoint, float armorPoint) { }

        public override void OnStateCollisionEnter(Collision collision) { }
        public override void OnStateTriggerEnter(Collider collider) { }
        public override void OnStateTriggerExit(Collider collider) { }
        public override void OnWeaponReceived(BaseWeapon weapon) { }

        public override void OnStateEnter() { }
        public override void OnStateUpdate() { }
        public override void OnStateExit() { }

        public override void OnHandleSit() { }
        public override void OnHandleStand() { }
        public override void OnHandleJump() { }
        public override void OnHandleMove(Vector3 input) { }
        public override void OnHandleStop() { }

        public override void OnHandleEquip(BaseWeapon.Type type) { }
        public override void OnHandleDrop() { }
        public override void OnHandleReload() { }
        public override void OnHandleInteract() { }

        public override void OnHandleEventStart(BaseWeapon.EventType type) { }
        public override void OnHandleEventEnd() { }

        public override void OnHandleEquipMainWeapon() { }
        public override void OnHandleEquipSubWeapon() { }
        public override void OnHandleEquipMeleeWeapon() { }

        public override void OnHandleBuildFormation() { }
        public override void OnHandleFreeRole() { }
    }

    abstract public class BaseState
    {
        public abstract void OnMessageReceived(string message, BaseWeapon.Type weaponType);
        public abstract void OnMessageReceived(string message, bool containSameType);
        public abstract void OnMessageReceived(string message, BaseWeapon newWeapon);
        public abstract void OnMessageReceived(string message, float multiplier);

        public abstract void CheckStateChange();
        public abstract void OnStateFixedUpdate();
        public abstract void OnStateLateUpdate();
        public abstract void OnNoiseReceived();
        public abstract void OnDamaged(float damage);
        public abstract void OnHeal(float hpPoint, float armorPoint);


        public abstract void OnStateCollisionEnter(Collision collision);
        public abstract void OnStateTriggerEnter(Collider collider);
        public abstract void OnStateTriggerExit(Collider collider);
        public abstract void OnWeaponReceived(BaseWeapon weapon);

        public abstract void OnStateEnter();
        public abstract void OnStateUpdate();
        public abstract void OnStateExit();

        public abstract void OnHandleSit();
        public abstract void OnHandleStand();
        public abstract void OnHandleJump();

        public abstract void OnHandleMove(Vector3 input);
        public abstract void OnHandleStop();

        public abstract void OnHandleEquip(BaseWeapon.Type type);
        public abstract void OnHandleDrop();
        public abstract void OnHandleReload();
        public abstract void OnHandleInteract();

        public abstract void OnHandleEventStart(BaseWeapon.EventType type);
        public abstract void OnHandleEventEnd();

        public abstract void OnHandleEquipMainWeapon();
        public abstract void OnHandleEquipSubWeapon();
        public abstract void OnHandleEquipMeleeWeapon();

        public abstract void OnHandleBuildFormation();
        public abstract void OnHandleFreeRole();
    }

    // 콜백 함수를 넣어놓는다.
    public class BaseMachine
    {
        //현재 상태를 담는 프로퍼티.
        protected BaseState _currentState;

        public void OnDamaged(float damage)
        {
            if (_currentState == null) return;
            _currentState.OnDamaged(damage);
        }

        public void OnHeal(float hpPoint, float armorPoint)
        {
            if (_currentState == null) return;
            _currentState.OnHeal(hpPoint, armorPoint);
        }

        public void OnWeaponReceived(BaseWeapon weapon)
        {
            if (_currentState == null) return;
            _currentState.OnWeaponReceived(weapon);
        }

        public void OnNoiseReceived()
        {
            if (_currentState == null) return;
            _currentState.OnNoiseReceived();
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (_currentState == null) return;
            _currentState.OnStateCollisionEnter(collision);
        }

        public void OnTriggerEnter(Collider collider)
        {
            if (_currentState == null) return;
            _currentState.OnStateTriggerEnter(collider);
        }

        public void OnTriggerExit(Collider collider)
        {
            if (_currentState == null) return;
            _currentState.OnStateTriggerExit(collider);
        }

        public void OnUpdate()
        {
            if (_currentState == null) return;
            _currentState.OnStateUpdate();
            _currentState.CheckStateChange();
        }

        public void OnFixedUpdate()
        {
            if (_currentState == null) return;
            _currentState.OnStateFixedUpdate();
        }

        public void OnLateUpdate()
        {
            if (_currentState == null) return;
            _currentState.OnStateLateUpdate();
        }

        // 키 이벤트 추가
        // 마우스 이벤트 추가

        public void OnHandleSit()
        {
            if (_currentState == null) return;
            _currentState.OnHandleSit();
        }

        public void OnHandleStand()
        {
            if (_currentState == null) return;
            _currentState.OnHandleStand();
        }

        public void OnHandleStop()
        {
            if (_currentState == null) return;
            _currentState.OnHandleStop();
        }

        public void OnHandleJump()
        {
            if (_currentState == null) return;
            _currentState.OnHandleJump();
        }

        public void OnHandleMove(Vector3 input)
        {
            if (_currentState == null) return;
            _currentState.OnHandleMove(input);
        }


        public void OnHandleEquip(BaseWeapon.Type type)
        {
            if (_currentState == null) return;
            _currentState.OnHandleEquip(type);
        }

        public void OnHandleEventStart(BaseWeapon.EventType type)
        {
            if (_currentState == null) return;
            _currentState.OnHandleEventStart(type);
        }

        public void OnHandleEventEnd()
        {
            if (_currentState == null) return;
            _currentState.OnHandleEventEnd();
        }

        public void OnHandleReload()
        {
            if (_currentState == null) return;
            _currentState.OnHandleReload();
        }

        public void OnHandleDrop()
        {
            if (_currentState == null) return;
            _currentState.OnHandleDrop();
        }

        public void OnHandleInteract()
        {
            if (_currentState == null) return;
            _currentState.OnHandleInteract();
        }

        public void OnHandleBuildFormation()
        {
            if (_currentState == null) return;
            _currentState.OnHandleBuildFormation();
        }

        public void OnHandleFreeRole()
        {
            if (_currentState == null) return;
            _currentState.OnHandleFreeRole();
        }
    }

    public class StateMachine<T> : BaseMachine
    {
        Dictionary<T, BaseState> _stateDictionary = new Dictionary<T, BaseState>();

        //현재 상태를 담는 프로퍼티.
        BaseState _previousState;
        T _currentStateName;
        public T CurrentStateName { get { return _currentStateName; } }
        
        public void Initialize(Dictionary<T, BaseState> stateDictionary)
        {
            _currentState = null;
            _previousState = null;

            _currentStateName = default;

            _stateDictionary = stateDictionary;
        }

        public bool RevertToPreviousState()
        {
            return ChangeState(_previousState);
        }

        #region SetState

        public bool SetState(T stateName)
        {
            _currentStateName = stateName;
            return ChangeState(_stateDictionary[stateName]);
        }

        public bool SetState(T stateName, string message, BaseWeapon.Type weaponType)
        {
            _currentStateName = stateName;
            return ChangeState(_stateDictionary[stateName], message, weaponType);
        }

        public bool SetState(T stateName, string message, BaseWeapon weapon)
        {
            _currentStateName = stateName;
            return ChangeState(_stateDictionary[stateName], message, weapon);
        }

        #endregion

        #region ChangeState

        bool ChangeState(BaseState state)
        {
            if (_stateDictionary.ContainsValue(state) == false) return false;

            if (_currentState == state) // 같은 State로 전환하지 못하게 막기
            {
                return false;
            }

            if (_currentState != null) //상태가 바뀌기 전에, 이전 상태의 Exit를 호출
                _currentState.OnStateExit();

            _previousState = _currentState;

            _currentState = state;


            if (_currentState != null) //새 상태의 Enter를 호출한다.
            {
                _currentState.OnStateEnter();
            }

            return true;
        }

        bool ChangeState(BaseState state, string message, BaseWeapon.Type weaponType)
        {
            if (_stateDictionary.ContainsValue(state) == false) return false;

            if (_currentState == state) // 같은 State로 전환하지 못하게 막기
            {
                return false;
            }

            if (_currentState != null) //상태가 바뀌기 전에, 이전 상태의 Exit를 호출
                _currentState.OnStateExit();

            _previousState = _currentState;

            _currentState = state;


            if (_currentState != null) //새 상태의 Enter를 호출한다.
            {
                _currentState.OnMessageReceived(message, weaponType);
                _currentState.OnStateEnter();
            }

            return true;
        }

        bool ChangeState(BaseState state, string message, bool isTrue)
        {
            if (_stateDictionary.ContainsValue(state) == false) return false;

            if (_currentState == state) // 같은 State로 전환하지 못하게 막기
            {
                return false;
            }

            if (_currentState != null) //상태가 바뀌기 전에, 이전 상태의 Exit를 호출
                _currentState.OnStateExit();

            _previousState = _currentState;

            _currentState = state;


            if (_currentState != null) //새 상태의 Enter를 호출한다.
            {
                _currentState.OnMessageReceived(message, isTrue);
                _currentState.OnStateEnter();
            }

            return true;
        }

        bool ChangeState(BaseState state, string message, BaseWeapon baseWeapon)
        {
            if (_stateDictionary.ContainsValue(state) == false) return false;

            if (_currentState == state) // 같은 State로 전환하지 못하게 막기
            {
                return false;
            }

            if (_currentState != null) //상태가 바뀌기 전에, 이전 상태의 Exit를 호출
                _currentState.OnStateExit();

            _previousState = _currentState;

            _currentState = state;


            if (_currentState != null) //새 상태의 Enter를 호출한다.
            {
                _currentState.OnMessageReceived(message, baseWeapon);
                _currentState.OnStateEnter();
            }

            return true;
        }

        #endregion
    }
}