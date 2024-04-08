using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    abstract public class State : BaseState
    {
        public override void CheckStateChange() { }

        public override void OnMessageReceived(BaseWeapon.Type weaponType) { } // 여러 개 만들어서 상속
        public override void OnMessageReceived(bool containSameType) { }

        public override void OnStateFixedUpdate() { }

        public override void OnStateLateUpdate() { }

        public override void OnNoiseReceived() { }

        public override void OnDamaged(float damage) { }


        public override void OnStateCollisionEnter(Collision collision) { }

        public override void OnStateTriggerEnter(Collider collider) { }

        public override void OnStateTriggerExit(Collider collider) { }


        public override void OnStateEnter() { }

        public override void OnStateUpdate() { }

        public override void OnStateExit() { }
    }

    abstract public class BaseState
    {
        public abstract void OnMessageReceived(BaseWeapon.Type weaponType);

        public abstract void OnMessageReceived(bool containSameType);

        public abstract void CheckStateChange();

        public abstract void OnStateFixedUpdate();

        public abstract void OnStateLateUpdate();

        public abstract void OnNoiseReceived();

        public abstract void OnDamaged(float damage);


        public abstract void OnStateCollisionEnter(Collision collision);

        public abstract void OnStateTriggerEnter(Collider collider);

        public abstract void OnStateTriggerExit(Collider collider);


        public abstract void OnStateEnter();

        public abstract void OnStateUpdate();

        public abstract void OnStateExit();

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
    }

    public class StateMachine<T> : BaseMachine
    {
        Dictionary<T, BaseState> _stateDictionary = new Dictionary<T, BaseState>();

        //현재 상태를 담는 프로퍼티.
        BaseState _previousState;
        
        public void Initialize(Dictionary<T, BaseState> stateDictionary)
        {
            _currentState = null;
            _previousState = null;

            _stateDictionary = stateDictionary;
        }

        public bool RevertToPreviousState()
        {
            return ChangeState(_previousState);
        }

        #region SetState

        public bool SetState(T stateName)
        {
            return ChangeState(_stateDictionary[stateName]);
        }

        public bool SetState(T stateName, BaseWeapon.Type weaponType)
        {
            return ChangeState(_stateDictionary[stateName], weaponType);
        }

        public bool SetState(T stateName, bool isTrue)
        {
            return ChangeState(_stateDictionary[stateName], isTrue);
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

        bool ChangeState(BaseState state, BaseWeapon.Type weaponType)
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
                _currentState.OnMessageReceived(weaponType);
                _currentState.OnStateEnter();
            }

            return true;
        }

        bool ChangeState(BaseState state, bool isTrue)
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
                _currentState.OnMessageReceived(isTrue);
                _currentState.OnStateEnter();
            }

            return true;
        }

        #endregion
    }
}