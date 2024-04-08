using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    [SerializeField] Noise _noisePrefab;
    [SerializeField] float _disableTime;
    [SerializeField] float _radius = 10;

    public void GenerateNoise(Vector3 pos)
    {
        Noise noise = ObjectPooler.SpawnFromPool<Noise>("Noise", pos);
        noise.Initialize(_radius, _disableTime);
    }
}
