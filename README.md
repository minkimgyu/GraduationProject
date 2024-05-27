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

* ### 핵심 기능
--> 여기에 핵심 기능 링크 넣어주기

* ### FSM을 활용한 Player 제작

   <div align="center">
    <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/3c82aae5-01d4-45c6-8ed9-c6b9378da567" width="100%" height="100%"/>
  
  </div>

  

  이동, 자세, 상호작용, 무기 사용 기능을 구현하기 위해 각각의 기능을 독립시켜 복잡도를 낮추기 위해 Concurrent State Machine을 적용했습니다. 앞으로의 확장성을 위해 Hierachical Finite State Machine 방식을 통해 이동 기능을 구현했습니다.

   </br>

* ### State 패턴을 사용하여 무기 시스템 개발

* ### FSM, Behavior Tree를 활용한 AI 제작

FSM만으로 기능을 구성하기에 State가 너무 많아져서 유지보수가 힘들어지는 문제점이 있었습니다.

 <div align="center">
    <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/8fea1e45-d6d2-4ba6-a2c5-77b2fdf8ca8c" width="100%" height="100%"/>
 </div>



출처 영상: https://www.youtube.com/watch?v=BeqU-njZesY&t=2s

이를 해결하기 위해 Muse Behavior의 FSM과 Behavior Tree가 혼합된 기능을 참고하여 AI 구현에 적용했습니다.

   - 좀비 AI

<div align="center">
   <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/8f59dcfe-5924-4e60-8dde-8db4ce6de5a8" width="60%" height="60%"/>
   <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/8e1b5f56-59b8-40c0-bd7e-f99587944e84" width="80%" height="80%"/>
</div>

각 State 하위에 Behavior Tree를 추가하여 기능을 구현했습니다.


* ### A* 알고리즘을 활용한 길 찾기 시스템 개발 및 최적화

* ### Object Pool를 활용한 이펙트, 소음 생성 시스템 개발 및 최적화



* ### Factory 패턴을 사용한 생성 시스템 개발

* ### Command 패턴을 활용한 조력자 이벤트 시스템 개발

* ### UI Toolkit를 사용하여 반동 커스텀 에디터 개발

* ### Rig Builder Package를 사용하여 IK 장전 애니메이션 적용


## 회고

