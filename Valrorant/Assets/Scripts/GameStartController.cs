using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class GameStartController : MonoBehaviour
{
    [SerializeField] Button _startButton;
    [SerializeField] Button _exitButton;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        _startButton.onClick.AddListener(StartGame);
        _exitButton.onClick.AddListener(ExitGame);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("FinalScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
