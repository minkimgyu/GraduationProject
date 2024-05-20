using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    System.Func<CharacterPlant.Name, Vector3, GameObject> Spawn;

    // Start is called before the first frame update
    void Start()
    {
        Spawn = FindObjectOfType<CharacterPlant>().Create;
        CharacterPlant plant = FindObjectOfType<CharacterPlant>();
        Spawn(CharacterPlant.Name.Player, transform.position);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
    }
}
