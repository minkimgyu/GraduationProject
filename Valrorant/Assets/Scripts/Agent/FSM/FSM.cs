using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    public interface IState
    {
        void CheckStateChange();

        void OnStateEnter();

        void OnStateUpdate();

        void OnStateFixedUpdate();

        void OnStateLateUpdate();

        void OnStateCollisionEnter(Collision collision);

        void OnStateExit();
    }

    public class StateMachine<T>
    {
        Dictionary<T, IState> _stateDictionary = new Dictionary<T, IState>();

        //현재 상태를 담는 프로퍼티.
        IState _currentState;
        IState _previousState;

        public void Initialize(Dictionary<T, IState> stateDictionary, T defaultState)
        {
            _currentState = null;
            _previousState = null;

            _stateDictionary = stateDictionary;
            SetState(defaultState);
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (_currentState == null) return;
            _currentState.OnStateCollisionEnter(collision);
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

        public bool RevertToPreviousState()
        {
            return SetState(_previousState);
        }

        public bool SetState(T stateName)
        {
            if (_stateDictionary.ContainsKey(stateName) == false) return false;

            if (_currentState == _stateDictionary[stateName]) // 같은 State로 전환하지 못하게 막기
            {
                return false;
            }

            if (_currentState != null) //상태가 바뀌기 전에, 이전 상태의 Exit를 호출한다.
                _currentState.OnStateExit();

            _previousState = _currentState;

            _currentState = _stateDictionary[stateName];

            if (_currentState != null) //새 상태의 Enter를 호출한다.
                _currentState.OnStateEnter();

            return true;
        }

        bool SetState(IState state)
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
                _currentState.OnStateEnter();

            return true;
        }
    }
}