using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType
{
    Sound,
    Human,
    Zombie
}

public interface ITarget
{
    TargetType MyType { get; set; }
    Transform ReturnTransform();


    Vector3 ReturnPos();
}

public interface ISightTarget : ITarget
{
    /// <summary>
    /// 시아를 통해 탐지해야하는 타겟의 경우 필요한 시아 포인트
    /// </summary>
    Transform ReturnSightPoint();
}

public interface IFlockingTarget : ITarget
{
    /// <summary>
    /// FollowFSM의 State가 Leader인 경우
    /// </summary>
    bool IsLeader();

    /// <summary>
    ///  같은 목표를 가지고 있을 때
    /// </summary>
    bool HasSameTarget(ITarget target);
}