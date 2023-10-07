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

        //���� ���¸� ��� ������Ƽ.
        IState _currentState;
        IState _previousState;

        //�⺻ ���¸� �����ÿ� �����ϰ� ������ �����.
        public void Initialize(Dictionary<T, IState> stateDictionary, T defaultState) // ���� �Լ� ó��
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

        //�ܺο��� ������¸� �ٲ��ִ� �κ�.
        public bool SetState(T stateName)//IState<T, W> state) // �߻� �Լ� ó��
        {
            if (_stateDictionary.ContainsKey(stateName) == false) return false;

            if (_currentState == _stateDictionary[stateName]) // ���� State�� ��ȯ���� ���ϰ� ����
            {
                Debug.Log("���� �̹� �ش� �����Դϴ�.");
                return false;
            }

            if (_currentState != null) //���°� �ٲ�� ����, ���� ������ Exit�� ȣ���Ѵ�.
                _currentState.OnStateExit();

            _previousState = _currentState;

            //���� ��ü.
            _currentState = _stateDictionary[stateName];

            if (_currentState != null) //�� ������ Enter�� ȣ���Ѵ�.
                _currentState.OnStateEnter();

            return true;
        }

        bool SetState(IState state)//IState<T, W> state) // �߻� �Լ� ó��
        {
            if (_stateDictionary.ContainsValue(state) == false) return false;

            if (_currentState == state) // ���� State�� ��ȯ���� ���ϰ� ����
            {
                Debug.Log("���� �̹� �ش� �����Դϴ�.");
                return false;
            }

            if (_currentState != null) //���°� �ٲ�� ����, ���� ������ Exit�� ȣ���Ѵ�.
                _currentState.OnStateExit();

            _previousState = _currentState;

            //���� ��ü.
            _currentState = state;

            if (_currentState != null) //�� ������ Enter�� ȣ���Ѵ�.
                _currentState.OnStateEnter();

            return true;
        }
    }
}