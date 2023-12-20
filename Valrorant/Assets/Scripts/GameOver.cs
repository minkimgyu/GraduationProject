using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    string fileName = "StageData.txt";
    string filePath;

    void Start()
    {
        filePath = Application.persistentDataPath + "/" + fileName;
    }

    bool RemoveFile()
    {
        if (File.Exists(filePath) == false) return false; // ������ ���ٸ� �������ش�.

        File.Delete(filePath);
        return true;
    }

    public void RestartGame()
    {
        bool canRemove = RemoveFile();
        if (canRemove == false) return;

        SceneManager.LoadScene("PlayScene");
        // �ٽ� ���� ������ ������
    }

    public void ReturnToMenu()
    {
        bool canRemove = RemoveFile();
        if (canRemove == false) return;

        SceneManager.LoadScene("MenuScene");
    }

    public void ExitGame()
    {
        bool canRemove = RemoveFile();
        if (canRemove == false) return;

        Application.Quit();
    }
}
