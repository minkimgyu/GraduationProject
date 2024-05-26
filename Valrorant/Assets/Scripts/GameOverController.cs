using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [SerializeField] Button _restartButton;
    [SerializeField] Button _returnToMenuButton;
    [SerializeField] Button _exitButton;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        _restartButton.onClick.AddListener(Restart);
        _returnToMenuButton.onClick.AddListener(ReturnToMenu);
        _exitButton.onClick.AddListener(ExitGame);
    }

    void Restart()
    {
        SceneManager.LoadScene("FinalScene");
    }

    void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    void ExitGame()
    {
        Application.Quit();
    }
}
