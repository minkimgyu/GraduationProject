using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CharacterPlant plant = FindObjectOfType<CharacterPlant>();
        Transform player = plant.Create(CharacterPlant.Name.Player);
        player.position = new Vector3(0, -0.5f, 0);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
    }
}
