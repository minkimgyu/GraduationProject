using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
struct StageData
{
    [SerializeField]
    int _playerWinCount;
    public int PlayerWinCount { get { return _playerWinCount; } set { _playerWinCount = value; } }

    [SerializeField]
    int _enemyWinCount;
    public int EnemyWinCount { get { return _enemyWinCount; } set { _enemyWinCount = value; } }
}

public class StageManager : MonoBehaviour
{
    string fileName = "StageData.txt";
    string filePath;

    [SerializeField] TMP_Text _myScore;
    [SerializeField] TMP_Text _enemyScore;

    // ������ �ִٸ� �ʱ� ������ ������ �����ؼ� �־��ش�. --> �̰Ŵ� �� �������� ����

    // Start is called before the first frame update
    void Start()
    {
        filePath = Application.persistentDataPath + "/" + fileName;
        Debug.Log(filePath);

        if (File.Exists(filePath) == true)
        {
            StageData data = ReturnStageData();
            ResetScore(data);
        }
        else
        {
            StageData stageData = new StageData();
            string initData = JsonUtility.ToJson(stageData);

            File.WriteAllText(filePath, initData); // �̷� ������� ����������

            ResetScore(stageData);
        }
    }

    void ResetScore(StageData data)
    {
        _myScore.text = data.PlayerWinCount.ToString();
        _enemyScore.text = data.EnemyWinCount.ToString();
    }

    StageData ReturnStageData()
    {
        string jdata = File.ReadAllText(filePath);
        return JsonUtility.FromJson<StageData>(jdata); // �����͸� �о�´�
    }

    void CheckGameOver(StageData stageData)
    {
        if (stageData.PlayerWinCount - stageData.EnemyWinCount >= 3)
        {
            // ���� ����
            // ��� ������ --> �� ������ ������ �ʱ�ȭ �������
            SceneManager.LoadScene("WinScene");
        }
        else if(stageData.EnemyWinCount - stageData.PlayerWinCount >= 3)
        {
            SceneManager.LoadScene("DefeatScene");
            // ���� ����
            // �й� ������ --> �� ������ ������ �ʱ�ȭ �������
        }
        else // ������ ���� ���� ���
        {
            // ������ ����
            string jData = JsonUtility.ToJson(stageData);
            File.WriteAllText(filePath, jData); // �̷� ������� ����������

            SceneManager.LoadScene("PlayScene"); // �� �����
        }
    }

    public void OnPlayerKillRequested()
    {
        if (File.Exists(filePath) == false) return; // ������ ���ٸ� �������ش�.

        StageData stageData = ReturnStageData();
        stageData.EnemyWinCount += 1;

        CheckGameOver(stageData);
    }

    public void OnEnemyKillRequested()
    {
        if (File.Exists(filePath) == false) return; // ������ ���ٸ� �������ش�.

        StageData stageData = ReturnStageData();
        stageData.PlayerWinCount += 1;

        CheckGameOver(stageData);
    }
}
