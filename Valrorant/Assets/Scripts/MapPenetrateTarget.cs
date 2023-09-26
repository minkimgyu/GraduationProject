using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPenetrateTarget : MonoBehaviour, IPenetrateTarget
{
    [SerializeField]
    float durability;

    public float ReturnDurability()
    {
        return durability;
    }
}
