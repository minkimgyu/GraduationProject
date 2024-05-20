using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAreaEnter : MonoBehaviour
{
    [SerializeField] EnemySpawner[] spawners;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            for (int i = 0; i < spawners.Length; i++)
            {
                spawners[i].ActivateSpawn();
            }
        }
    }
}
