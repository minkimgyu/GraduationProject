using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
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

    public void StartGame()
    {
        RemoveFile();
        SceneManager.LoadScene("PlayScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
