using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GWorld : MonoBehaviour
{
    private static GWorld _instance = new GWorld();
    private static WorldStates _world;

    static GWorld()
    {
        _world = new WorldStates();
    }

    private GWorld()
    {
    }

    public static GWorld Instance { get { return _instance; } }

    public WorldStates World { get { return _world; } }
}
