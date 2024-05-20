using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[System.Serializable]
public class CharacterData
{
    public float maxHp;
    public float maxArmor;
}

[System.Serializable]
public class PlayerData
{
    public Database.PersonName personName;
    public float maxHp = 100;
    public float maxArmor = 100;

    public float viewYRange = 70;
    public SerializableVector2 viewSensitivity = new SerializableVector2(1000, 1000);

    public int helperCount = 3;
    public float startRange = 5f;

    public float weaponThrowPower = 5;

    public float walkSpeed = 80;
    public float walkSpeedOnAir = 40;
    public float jumpSpeed = 20;

    public float postureSwitchDuration = 1f;
    public float capsuleStandCenter = 1f;
    public float capsuleCrouchHeight = 1.7f;

    public float capsuleStandHeight = 2f;
    public float capsuleCrouchCenter = 1.15f;
}

public class PlayerFactory : CharacterFactory<PlayerData>
{
    public override GameObject Create(Vector3 pos)
    {
        Player character = Object.Instantiate(_prefab, pos, Quaternion.identity).GetComponent<Player>();
        character.Initialize(_data);
        return character.gameObject;
    }
}
