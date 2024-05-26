using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] float _spawnDelay;
    [SerializeField] float _radius;

    [SerializeField] BaseDrawer _baseDrawer;

    StopwatchTimer _timer;

    System.Func<CharacterPlant.Name, Vector3, GameObject> SpawnEnemy;
    CharacterPlant.Name[] _zombieNames = 
    { 
        CharacterPlant.Name.Police, 
        CharacterPlant.Name.Mask, 
        CharacterPlant.Name.Mild, 
        CharacterPlant.Name.Witch 
    };


    private void Start()
    {
        SpawnEnemy = FindObjectOfType<CharacterPlant>().Create;
        ActivateSpawn();
    }

    private void OnValidate()
    {
        _baseDrawer.ResetData(_radius);
    }

    public void ActivateSpawn()
    {
        _timer = new StopwatchTimer();
        _timer.Start(_spawnDelay);
    }

    public void DisableSpawn()
    {
        _timer = null;
    }

    private void Update()
    {
        if (_timer == null) return;

        int randomIndex = Random.Range(0, _zombieNames.Length);

        if(_timer.CurrentState == StopwatchTimer.State.Finish)
        {
            Vector2 posInCircle = Random.insideUnitCircle * _radius;

            SpawnEnemy(_zombieNames[randomIndex], new Vector3(posInCircle.x + transform.position.x, transform.position.y - 0.5f, posInCircle.y + transform.position.z));
            _timer.Reset();
            _timer.Start(_spawnDelay);
        }
    }
}
