using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAreaExit : MonoBehaviour
{
    [SerializeField] EnemySpawner[] spawners;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < spawners.Length; i++)
            {
                spawners[i].DisableSpawn();
            }
        }
    }
}
