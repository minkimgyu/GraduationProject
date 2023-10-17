using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FracturedObject : MonoBehaviour
{
    public void Initialize(float duration) 
    {
        Destroy(gameObject, duration);
    }
}
