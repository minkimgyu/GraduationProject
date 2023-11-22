using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EventCallPart
{
    Both,
    Left,
    Right
}

public class RecoilStorage : MonoBehaviour
{
    [SerializeField]
    AssetDictionary weaponRecoilDictionary;

    JsonAssetGenerator jsonAssetGenerator = new JsonAssetGenerator();

    public T OnRecoilDataSendRequested<T>(BaseWeapon.Name weaponName, EventCallPart part)
    {
        return jsonAssetGenerator.JsonToObject<T>(weaponRecoilDictionary[weaponName][part]);
    }
}
