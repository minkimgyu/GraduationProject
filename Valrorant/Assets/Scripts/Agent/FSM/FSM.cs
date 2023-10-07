using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    public interface IState
    {
        void OnStateEnter();
        void OnStateUpdate();
        void OnStateFixedUpdate();
        void OnStateLateUpdate();
        void OnStateExit();
    }

    public class StateMachine<T>
    {
        Dictionary<T, IState> _stateDictionary = new Dictionary<T, IState>();

        //현재 상태를 담는 프로퍼티.
        IState _currentState;
        IState _previousState;

        //기본 상태를 생성시에 설정하게 생성자 만들기.
        public void Initialize(Dictionary<T, IState> stateDictionary, T defaultState) // 가상 함수 처리
        {
            _currentState = null;
            _previousState = null;

            _stateDictionary = stateDictionary;
            SetState(defaultState);
        }

        public void DoUpdate()
        {
            if (_currentState == null) return;
            _currentState.OnStateUpdate();
        }

        public void DoFixedUpdate()
        {
            if (_currentState == null) return;
            _currentState.OnStateFixedUpdate();
        }

        public void DoLateUpdate()
        {
            if (_currentState == null) return;
            _currentState.OnStateLateUpdate();
        }

        public bool RevertToPreviousState()
        {
            return SetState(_previousState);
        }

        //외부에서 현재상태를 바꿔주는 부분.
        public bool SetState(T stateName)//IState<T, W> state) // 추상 함수 처리
        {
            if (_stateDictionary.ContainsKey(stateName) == false) return false;

            if (_currentState == _stateDictionary[stateName]) // 같은 State로 전환되지 못하게 막기
            {
                Debug.Log("현재 이미 해당 상태입니다.");
                return false;
            }

            if (_currentState != null) //상태가 바뀌기 전에, 이전 상태의 Exit를 호출한다.
                _currentState.OnStateExit();

            _previousState = _currentState;

            //상태 교체.
            _currentState = _stateDictionary[stateName];

            if (_currentState != null) //새 상태의 Enter를 호출한다.
                _currentState.OnStateEnter();

            return true;
        }

        bool SetState(IState state)//IState<T, W> state) // 추상 함수 처리
        {
            if (_stateDictionary.ContainsValue(state) == false) return false;

            if (_currentState == state) // 같은 State로 전환되지 못하게 막기
            {
                Debug.Log("현재 이미 해당 상태입니다.");
                return false;
            }

            if (_currentState != null) //상태가 바뀌기 전에, 이전 상태의 Exit를 호출한다.
                _currentState.OnStateExit();

            _previousState = _currentState;

            //상태 교체.
            _currentState = state;

            if (_currentState != null) //새 상태의 Enter를 호출한다.
                _currentState.OnStateEnter();

            return true;
        }
    }
}