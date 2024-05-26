using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    System.Func<CharacterPlant.Name, Vector3, GameObject> Spawn;
    [SerializeField] Transform _spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        Spawn = FindObjectOfType<CharacterPlant>().Create;
        CharacterPlant plant = FindObjectOfType<CharacterPlant>();
        Spawn(CharacterPlant.Name.Player, _spawnPoint.position);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
    }
}
