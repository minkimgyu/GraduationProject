using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;
using System;
using Object = UnityEngine.Object;

[System.Serializable]
public class ZombieData
{
    public int moneyPerOne = 30;
    public float maxHp = 300;
    public float maxArmor = 50;

    public float destoryDelay = 5;

    public float angleOffset = 30;
    public float angleChangeAmount = 0.1f;
    public float stateChangeDelay = 7;

    public float moveSpeed = 3;
    public float viewSpeed = 5;
    public float attackDamage = 20;
    public int wanderOffset = 7;

    public float targetCaptureRadius = 8;
    public float targetCaptureAdditiveRadius = 1f;
    public float targetCaptureAngle = 90;

    public float noiseCaptureRadius = 11;
    public int maxNoiseQueueSize = 3;

    public float additiveAttackRadius = 0.3f;
    public float attackRange = 1.2f;
    public float attackCircleRadius = 1.5f;

    public float preAttackDelay = 0.5f;
    public float delayForNextAttack = 3;

    public float pathFindDelay = 0.5f;
}

public class ZombieFactory : CharacterFactory<ZombieData>
{
    Action<int> AddMoney;

    public override void Initialize(CharacterFactoryData data)
    {
        base.Initialize(data);
        AddMoney = Object.FindObjectOfType<Shop>().AddMoney;
    }

    public override GameObject Create(Vector3 pos)
    {
        Zombie zombie = Object.Instantiate(_prefab, pos, Quaternion.identity).GetComponent<Zombie>();
        zombie.Initialize(_data, AddMoney);
        return zombie.gameObject;
    }
}
