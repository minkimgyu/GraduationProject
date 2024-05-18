using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Zombie _zombiePrefab;
    [SerializeField] float _spawnDelay;
    [SerializeField] float _radius;
    [SerializeField] float _yPos = 0.5f;

    [SerializeField] BaseDrawer _baseDrawer;

    StopwatchTimer _timer;

    private void Start()
    {
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
        Zombie zombie = Instantiate(_zombiePrefab, pos, Quaternion.identity);
        //zombie.Initialize();
    }
}
