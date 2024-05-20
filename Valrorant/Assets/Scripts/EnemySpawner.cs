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

    private void Start()
    {
        SpawnEnemy = FindObjectOfType<CharacterPlant>().Create;
        _timer = new StopwatchTimer();
        ActivateSpawn();
    }

    private void OnValidate()
    {
        _baseDrawer.ResetData(_radius);
    }

    public void ActivateSpawn()
    {
        if (_timer == null) return;

        _timer.Start(_spawnDelay);
    }

    public void DisableSpawn()
    {
        _timer = null;
    }

    private void Update()
    {
        if (_timer == null) return;

        if(_timer.CurrentState == StopwatchTimer.State.Finish)
        {
            Vector2 posInCircle = Random.insideUnitCircle * _radius;

            Spawn(new Vector3(posInCircle.x + transform.position.x, transform.position.y, posInCircle.y + transform.position.z));
            _timer.Reset();
            _timer.Start(_spawnDelay);
        }
    }

    void Spawn(Vector3 pos)
    {
        Vector2 startPos = Random.insideUnitCircle * _radius;
        SpawnEnemy(CharacterPlant.Name.Zombie, pos + new Vector3(startPos.y, startPos.x));
    }
}
