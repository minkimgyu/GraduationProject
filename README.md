# Zombie Factory

![ZF](https://github.com/minkimgyu/GraduationProject/assets/48249824/d4a7accb-8bf7-4e6b-a8cf-ca28b6830452)
참고 영상: https://www.youtube.com/watch?v=Bqul9EeTsTc

## 프로젝트 소개
Unity를 사용하여 개발한 FPS 게임

## 개발 기간
23. 09 ~ 24. 05

## 인원
1인 개발

## 개발 환경
Unity (C#)

## 기능 설명

### FSM을 활용한 Player 구현
</br>
<div align="center">
   <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/ccd318f4-0b3b-4eb4-8866-af6f988f5084" width="100%" height="100%"/>
   </br>
   <a href="https://github.com/minkimgyu/GraduationProject/blob/f9a92a93654f787c2032804344c8a420ec8bc562/Valrorant/Assets/Scripts/Agent/ActionController.cs#L93">Movement, Posture FSM 코드 보러가기</a>
   </br>
   <a href="https://github.com/minkimgyu/GraduationProject/blob/f9a92a93654f787c2032804344c8a420ec8bc562/Valrorant/Assets/Scripts/Agent/InteractionController.cs#L47">Interaction FSM 코드 보러가기</a>
   </br>
   <a href="https://github.com/minkimgyu/GraduationProject/blob/f9a92a93654f787c2032804344c8a420ec8bc562/Valrorant/Assets/Scripts/WeaponController.cs#L220">Weapon FSM 코드 보러가기</a>
</div>
</br> 

   * 플레이어의 기능을 구현하기 위해 각각의 기능을 독립시켜 Concurrent State Machine을 적용했습니다.
   * 앞으로의 확장성을 위해 Hierachical Finite State Machine 방식을 통해 Movement FSM을 구현했습니다.
</br>

### FSM, Behavior Tree를 활용한 AI 구현

FSM만으로 AI를 구현하기에 State가 너무 많아져서 유지보수가 힘들어지는 문제점이 있었습니다.

 <div align="center">
    <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/8fea1e45-d6d2-4ba6-a2c5-77b2fdf8ca8c" width="100%" height="100%"/>
 </div>

이를 해결하기 위해 Unity Muse Behavior를 참고하여 FSM과 Behavior Tree가 혼합하여 AI 기능을 구현했습니다.

출처 영상: https://www.youtube.com/watch?v=BeqU-njZesY&t=2s

</br>

### 좀비 AI

<div align="center">
   <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/1fd78ef5-e7b7-4dd8-82ec-f857d11d5a85" width="60%" height="60%"/>
</div>
<div align="center">
   Idle, TargetFollow State에 Behavior Tree를 구현하여 기능을 개발했습니다.
</div>

</br>
<details>
   <summary>State 전이 조건</summary>
   <div align="center">
      <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/50ce25b9-8571-46be-ae44-fc15c0970170" width="80%" height="80%"/>
   </div>
</details>

<details>
   <summary>Idle State</summary>

   <div align="center">
      <a href="https://github.com/minkimgyu/GraduationProject/blob/83793c3f3e063f4d9e2b7ad62e0ca9b39228e8b0/Valrorant/Assets/Scripts/AI/Zombie/States/Idle/IdleState.cs#L13">코드 보러가기</a>
      </br>
      <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/b9c4c3df-992a-4fe9-b07f-53c0f83cb952" width="80%" height="80%"/>
      </br>
      주변을 배회하는 기능을 구현했습니다.
   </div>
</details>

<details>
   <summary>TargetFollow State</summary>
      <div align="center">
      <a href="https://github.com/minkimgyu/GraduationProject/blob/ed2ec6f107ff966a7e3540d05ee64a8e2538e2af/Valrorant/Assets/Scripts/AI/Zombie/States/Following/TargetFollowingState.cs#L13">코드 보러가기</a>
      </br>
      <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/d233ac76-5c56-4155-b752-ec5075449a40" width="80%" height="80%"/>
      </br>     
      Target을 추적하여 공격하는 기능을 구현했습니다.
   </div>
</details>


<details>
   <summary>NoiseTracking State</summary>
   <div align="center">
      <a href="https://github.com/minkimgyu/GraduationProject/blob/ed2ec6f107ff966a7e3540d05ee64a8e2538e2af/Valrorant/Assets/Scripts/AI/Zombie/States/Tracking/NoiseTrackingState.cs#L10">코드 보러가기</a>
      </br>
      <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/db2fe0fa-7254-4aca-affb-923ad8bbd6c8" width="80%" height="80%"/>
      </br>
      주변에 소음을 탐지하여 추적하는 기능을 구현했습니다.
      </br>
      가장 먼저 탐지한 Noise 추적하기 위해서 Queue(FIFO)를 사용하여 Noise를 관리했습니다.
   </div>
</details>

</br>

### 조력자 AI

<div align="center">
   <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/4c72c1fd-9127-4d93-a48d-e207f5460628" width="80%" height="80%"/>


</div>

- FreeRole State 내부에 FSM을 구현하여 Hierachical Finite State Machine 방식을 적용했습니다.
- Concurrent State Machine을 적용하여 이동 로직과 공격 로직을 나눠서 복잡도를 줄였습니다.
- Exploring, Combat, BuildFormation State에 Behavior Tree를 구현하여 기능을 개발했습니다.

</br>

<details>
   <summary>State 전이 조건</summary>
   <div align="center">
      <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/e5f84165-bf0c-4d25-a987-71545699666b" width="80%" height="80%"/>
   </div>
</details>

<details>
   <summary>Exploring State</summary>
   
   <div align="center">
      <a href="https://github.com/minkimgyu/GraduationProject/blob/a5308f9bc5f08008d5e2b576e4ce0b5c4686e830/Valrorant/Assets/Scripts/AI/Swat/States/Movement/FreeRole/ExploringState.cs#L13">코드 보러가기</a>
      </br>
      <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/7cb971c5-dda5-4097-b6b4-eb3eb39f2c69" width="80%" height="80%"/>
      </br>
      주변을 배회하는 기능을 구현했습니다.
   </div>
</details>

<details>
   <summary>Combat State</summary>
      <div align="center">
      <a href="https://github.com/minkimgyu/GraduationProject/blob/a5308f9bc5f08008d5e2b576e4ce0b5c4686e830/Valrorant/Assets/Scripts/AI/Swat/States/Movement/FreeRole/CombatState.cs#L13">코드 보러가기</a>
      </br>
      <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/f385dc73-97e5-439e-992a-cfbe7a49f9e5" width="50%" height="50%"/>
      </br>     
      Target을 조준하는 기능을 구현했습니다.
   </div>
</details>

<details>
   <summary>BuildFormation State</summary>
   <div align="center">
      <a href="https://github.com/minkimgyu/GraduationProject/blob/a5308f9bc5f08008d5e2b576e4ce0b5c4686e830/Valrorant/Assets/Scripts/AI/Swat/States/Movement/BuildingFormation/BuildingFormationState.cs#L13">코드 보러가기</a>
      </br>
      <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/a3d4bf0b-fd76-4942-8a3c-6419d7e21dca" width="50%" height="50%"/>
      </br>
      플레이어 주변으로 조력자를 위치시키는 기능을 구현했습니다.
   </div>
</details>

<details>
   <summary>Battle FSM</summary>
   <div align="center">
      <a href="https://github.com/minkimgyu/GraduationProject/blob/a5308f9bc5f08008d5e2b576e4ce0b5c4686e830/Valrorant/Assets/Scripts/AI/Swat/Helper.cs#L187">코드 보러가기</a>
      </br>
      조력자 AI의 공격 기능을 구현했습니다.
   </div>
</details>

</br>


### A* 알고리즘을 활용한 길 찾기 시스템 개발 및 최적화

#### 문제점

<div align="center">
   <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/e324928c-16b9-46f8-9805-d6c5afac6281" width="85%" height="85%"/>
   <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/c6eee27a-78bf-47c5-91d8-e9770fe2ddc9" width="85%" height="85%"/>
</div>

<div>
   유닛을 맵의 끝에서 끝으로 가는 경로를 길찾기 알고리즘을 적용하여 찾았을 때 약 1초 가까이 걸리는 것을 확인할 수 있습니다. 이는 너무 느리기 때문에 실제 게임에 적용하기 힘듭니다. 
</div>

#### 해결 과정

<div>
   A* 알고리즘은 가중치가 가장 작은 노드를 선택해 길찾기를 하는 특성이 있습니다. 기존에는 List를 통해 순회하면서 가중치가 가장 작은 값을 찾았기 때문에 시간 복잡도가 O(n)이었습니다.
   </br>
   </br>
   그러나 Min Heap을 사용한다면 삽입의 경우 시간복잡도가 O(logn)이지만 알고리즘의 특성 상 가장 가중치가 작은 노드를 언제나 트리의 최상단에 위치되므로 가중치가 가장 작은 노드에 시간 복잡도가 O(1)안에 접근할 수 있습니다.
   </br>
   </br>
   <div align="center">
   이러한 특성을 사용하여 최적화를 진행했습니다.
   </div>
</div>

<div align="center">
   <a href="https://github.com/minkimgyu/GraduationProject/blob/8de6eed4a8071a91e97da948b970a158ca5c180f/Valrorant/Assets/Scripts/Grid/Heap.cs#L12">Min Heap 코드 보러가기</a>
</div>
</br>
<div align="center">
   Min Heap 적용
   </br>
  <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/70af2e2b-e7c6-4f7b-a04f-9afe66244cfb" width="85%" height="85%"/>
</div>

</br>
<div align="center">
   거의 1초에 가깝던 수행 시간이 1/10 가까이 감소한 것을 확인할 수 있습니다.
</div>


### Object Pool를 사용한 Effect 생성 시스템 개발 및 최적화
#### 문제점

<div align="center">
   평상시
   </br>
   <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/beb36af6-d609-49ba-b960-8dd3ae12a001" width="55%" height="55%"/>

   사격 시
   </br>
  <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/58d2bba1-a9ea-41a9-ab22-cf30c07f69d6" width="55%" height="55%"/>
  <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/4d1294fb-baac-4da4-8cfd-ed6043fb720a" width="55%" height="55%"/>
</div>

<div>
   총기를 발사할 때 Effect와 Noise가 계속 생성되고 파괴되기 때문에 GC Spike가 많이 일어나며 프레임이 249에서 145까지 떨어지는 것을 확인할 수 있습니다.
</div>

#### 해결 과정

<div align="center">
   <a href="https://github.com/minkimgyu/GraduationProject/blob/c465c7a58a8f202b31801be35a3033ea732a1cd5/Valrorant/Assets/Scripts/ObjectPool.cs#L26">ObjectPool 코드 보러가기</a>
</div>
</br>

<div align="center">
   Object Pool을 이용하여 Effect와 Noise를 관리해줍나다.
</div>
</br>
<div align="center">
   Object Pool 적용
   </br>
  <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/cfbf2c71-292b-4e66-a131-ee587399fdca" width="55%" height="55%"/>
  <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/a4ad69ad-eeb0-4497-af08-da65a363aef9" width="55%" height="55%"/>
</div>

</br>
<div align="center">
   GC Spike가 눈에 띄게 줄어든 것을 볼 수 있으며 프레임도 220정도를 유지하는 것을 확인할 수 있습니다.
</div>

### Factory 패턴을 사용한 생성 시스템 개발

<div align="center">
   <a href="https://github.com/minkimgyu/GraduationProject/blob/e7e7c4bd03fe5930061d1ac35778dc7e14541275/Valrorant/Assets/Scripts/Factory/Weapon/WeaponPlant.cs#L31C23-L31C36">WeaponFactory 코드 보러가기</a>
   </br>
   <a href="https://github.com/minkimgyu/GraduationProject/blob/e7e7c4bd03fe5930061d1ac35778dc7e14541275/Valrorant/Assets/Scripts/Factory/Character/CharacterPlant.cs#L6">CharacterFactory 코드 보러가기</a>
</div>
</br>

```cs

abstract public class BaseFactory<T1, T2>
{
    protected JsonAssetGenerator _jsonAssetGenerator = new JsonAssetGenerator();

    public abstract void Initialize(T2 data);
    public virtual T1 Create() { return default; }
    public virtual T1 Create(Vector3 pos) { return default; }
    public virtual T1 Create(
        Vector3 pos,
        CharacterPlant.Name name,
        Func<Vector3> ReturnPlayerPos, 
        Action<BaseWeapon.Name> OnWeaponProfileChangeRequested,
        Action<float> OnHpChangeRequested,
        Action<CharacterPlant.Name> OnDieRequested
    ) 
    { return default; }

    public virtual T1 Create(
        Func<ShopBlackboard> ReturnBlackboard
    )
    { return default; }
}

```
<div align="center">
   Weapon과 Character(플레이어, 조력자, 좀비)를 Factory 패턴을 사용해 구현했습니다.
   </br>
   이를 통해 객체 간의 결합도를 낮췄습니다. 또한 Generic Programming을 적용하여 재사용성을 높혔습니다.
</div>

### Command 패턴을 사용한 입력 이벤트 시스템 개발

<div align="center">
   <a href="https://github.com/minkimgyu/GraduationProject/blob/e7e7c4bd03fe5930061d1ac35778dc7e14541275/Valrorant/Assets/Scripts/Input/InputHandler.cs#L6">InputHandler 코드 보러가기</a>
   </br>
   <a href="https://github.com/minkimgyu/GraduationProject/blob/e7e7c4bd03fe5930061d1ac35778dc7e14541275/Valrorant/Assets/Scripts/Agent/Command/BaseCommand.cs#L28">Command 코드 보러가기</a>
</div>
</br>

```cs

public class BaseCommand
{
    public BaseCommand(Action DoAction) { }
    public BaseCommand(Action<BaseWeapon> DoAction) { }
    public BaseCommand(Action<CharacterPlant.Name, BaseWeapon> DoAction) { }
    public BaseCommand(Action<CharacterPlant.Name> DoAction) { }
    public BaseCommand(Action<BaseWeapon.Type> DoAction) { }
    public BaseCommand(Action<BaseWeapon.EventType> DoAction) { }
    public BaseCommand(Action<Vector3> DoAction) { }
    public BaseCommand(Action<float> DoAction) { }

    public virtual void Execute() { }
    public virtual void Execute(BaseWeapon weapon) { }
    public virtual void Execute(CharacterPlant.Name name, BaseWeapon weapon) { }
    public virtual void Execute(CharacterPlant.Name name) { }
    public virtual void Execute(BaseWeapon.Type type) { }
    public virtual void Execute(BaseWeapon.EventType type) { }
    public virtual void Execute(Vector3 dir) { }
    public virtual void Execute(float hp) { }
}

```

</br>

<div align="center">
   실행될 기능을 캡슐화함으로써 유지보수와 확장성을 높혔습니다.
</div>

### UI Toolkit를 사용하여 반동 커스텀 에디터 개발

<div align="center">
   <a href="https://github.com/minkimgyu/GraduationProject/blob/d91d53c992ab21327b267b507454a9afceb68fd5/Valrorant/Assets/Editor/Map/SprayEditor.cs#L15">SprayEditor 코드 보러가기</a>
   </br>
   <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/5f8058ae-5c9e-4d3f-9e66-1ec668fc17be" width="100%" height="100%"/>

   <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/55fd198c-b728-4e39-9c37-4b859d71a024" width="100%" height="100%"/>
</div>
</br>

<div align="center">
   총기 밸런싱 작업을 효율적으로 하기 위해 UI ToolKit Package를 사용하여 반동 에디터를 개발해봤습니다.
</div>

### Rig Builder Package를 사용하여 IK 장전 애니메이션 적용

<div align="center">
   <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/c0eb37c8-92bd-4fa6-94df-e712123e9554" width="50%" height="50%"/>
   </br>
   적용 전
   </br>
   </br>
   <img src="https://github.com/minkimgyu/GraduationProject/assets/48249824/c8d42e5e-06e9-440b-8514-9e3f929dba57" width="50%" height="50%"/>
   </br>
   적용 후
</div>
</br>

<div align="center">
   Rig Builder Package를 활용해서 원본 애니메이션에 Inverse Kinematics를 적용해 수정하는 작업을 해봤습니다.
</div>

## 회고
졸업 프로젝트를 진행하면서 많은 부분을 새롭게 배우고 적용해봤습니다. 지금까지 했던 프로젝트 중에서 가장 많은 것을 얻어갈 수 있던 시간이었습니다.
