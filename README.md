# Zombie Factory

## 프로젝트 소개
Unity를 사용하여 개발한 FPS 게임

## 개발 기간
23.09. ~ 24.05.

## 인원
1인 개발

## 개발 환경
* Unity (C#)

## 기능 설명

* FSM을 활용한 Player 제작

   <div align="center">
    <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/3c82aae5-01d4-45c6-8ed9-c6b9378da567" width="100%" height="100%"/>
  
  </div>

  

  이동, 자세, 상호작용, 무기 사용 기능을 구현하기 위해 각각의 기능을 독립시켜 복잡도를 낮추기 위해 Concurrent State Machine을 적용해봤습니다. 또한 이동 기능 구현 시 앞으로 추가될 기능에 대비하여 Hierachical Finite State Machine 방식으로 구현해봤습니다.

   ```cs
    // FSM.cs
    public class StateMachine<T> : BaseMachine
    {

        abstract public class BaseState
        {
          ...
        }

        Dictionary<T, BaseState> _stateDictionary = new Dictionary<T, BaseState>();

        //현재 상태를 담는 프로퍼티.
        BaseState _previousState;
        
        public void Initialize(Dictionary<T, BaseState> stateDictionary)
        {
            _currentState = null;
            _previousState = null;

            _currentStateName = default;

            _stateDictionary = stateDictionary;
        }
      ...
   }
   ```
   State 패턴을 활용한 FSM을 적용하여 

* FSM, Behavior Tree를 활용한 AI 제작


* A* 알고리즘을 활용한 길 찾기 시스템 개발 및 최적화

* Object Pool를 활용한 이펙트, 소음 생성 시스템 개발 및 최적화

* State 패턴을 사용하여 무기 시스템 개발

* Factory 패턴을 사용한 생성 시스템 개발

* Command 패턴을 활용한 조력자 이벤트 시스템 개발

* UI Toolkit를 사용하여 반동 커스텀 에디터 개발

* Rig Builder Package를 사용하여 IK 장전 애니메이션 적용


## 회고

