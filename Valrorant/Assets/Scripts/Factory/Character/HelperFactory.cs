using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

[System.Serializable]
public class HelperData
{
    public float maxHp = 100;
    public float maxArmor = 50;

    public float destoryDelay = 5;

    public float angleOffset = 90;
    public float angleChangeAmount = 0.01f;

    public float weaponThrowPower = 5;

    public int wanderOffset = 5;
    public float stateChangeDelay = 5;

    public float moveSpeed = 3;
    public float viewSpeed = 5;

    public float largeTargetCaptureRadius = 8;
    public float largeTargetCaptureAngle = 180;

    public float smallTargetCaptureRadius = 3;
    public float smallTargetCaptureAngle = 360; // 이건 전 범위로 해줘야 한다.

    public float pathFindDelay = 0.5f;

    public float farFromPlayerDistance = 9;
    public float farFromPlayerDistanceOffset = 6;

    public float farFromTargetDistance = 6;
    public float farFromTargetDistanceOffset = 1;

    public float closeDistance = 2.5f;
    public float closeDistanceOffset = 0.5f;

    public float attackDelay = 3;
    public float attackDuration = 1;

    public float formationRadius = 10;

    public float formationOffset = 3;
    public float formationOffsetChangeDuration = 5;
}

public class HelperFactory : CharacterFactory<HelperData>
{
    public override Transform Create()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj == null) return null;

        Player player = playerObj.GetComponent<Player>();
        if (player == null) return null;

        Helper helper = Object.Instantiate(_prefab).GetComponent<Helper>();
        helper.Initialize(_data, player.ReturnPos);
        return helper.transform;
    }
}
