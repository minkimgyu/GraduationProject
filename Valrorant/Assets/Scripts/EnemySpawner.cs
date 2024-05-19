using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] float _spawnDelay;
    [SerializeField] float _radius;
    [SerializeField] float _yPos = 0.5f;

    [SerializeField] BaseDrawer _baseDrawer;

    StopwatchTimer _timer;

    System.Func<CharacterPlant.Name, Transform> SpawnEnemy;

    private void Start()
    {
        SpawnEnemy = FindObjectOfType<CharacterPlant>().Create;

        _timer = new StopwatchTimer();
        _timer.Start(_spawnDelay);
    }

    private void OnValidate()
    {
        _baseDrawer.ResetData(_radius);
    }

    private void Update()
    {
        if(_timer.CurrentState == StopwatchTimer.State.Finish)
        {
            Vector2 posInCircle = Random.insideUnitCircle * _radius;

            Spawn(new Vector3(posInCircle.x + transform.position.x, -_yPos, posInCircle.y + transform.position.z));
            _timer.Reset();
            _timer.Start(_spawnDelay);
        }
    }

    void Spawn(Vector3 pos)
    {
        Vector2 startPos = Random.insideUnitCircle * _radius;
        Transform zombie = SpawnEnemy(CharacterPlant.Name.Zombie);

        zombie.position = new Vector3(transform.position.x + pos.y, transform.position.y, transform.position.z + pos.x);
        zombie.rotation = Quaternion.identity;
    }
}
