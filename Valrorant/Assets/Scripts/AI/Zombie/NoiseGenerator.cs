using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    [SerializeField] Noise _noisePrefab;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            GenerateNoise(transform.position, 5);
        }
    }

    public void GenerateNoise(Vector3 pos, float radius)
    {
        pos += new Vector3(Random.Range(-15, 15), 0, Random.Range(-15, 15));
        Noise noise = Instantiate(_noisePrefab, pos, Quaternion.identity);
        noise.Initialize(radius, 3);
    }
}
