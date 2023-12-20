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

    // 파일이 있다면 초기 값으로 변수를 생성해서 넣어준다. --> 이거는 맨 마지막에 진행

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

            File.WriteAllText(filePath, initData); // 이런 방식으로 생성시켜줌

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
        return JsonUtility.FromJson<StageData>(jdata); // 데이터를 읽어온다
    }

    void CheckGameOver(StageData stageData)
    {
        if (stageData.PlayerWinCount - stageData.EnemyWinCount >= 3)
        {
            // 게임 종료
            // 우승 씬으로 --> 이 씬에서 데이터 초기화 해줘야함
            SceneManager.LoadScene("WinScene");
        }
        else if(stageData.EnemyWinCount - stageData.PlayerWinCount >= 3)
        {
            SceneManager.LoadScene("DefeatScene");
            // 게임 종료
            // 패배 씬으로 --> 이 씬에서 데이터 초기화 해줘야함
        }
        else // 결판이 나지 않은 경우
        {
            // 데이터 저장
            string jData = JsonUtility.ToJson(stageData);
            File.WriteAllText(filePath, jData); // 이런 방식으로 생성시켜줌

            SceneManager.LoadScene("PlayScene"); // 씬 재시작
        }
    }

    public void OnPlayerKillRequested()
    {
        if (File.Exists(filePath) == false) return; // 파일이 없다면 생생해준다.

        StageData stageData = ReturnStageData();
        stageData.EnemyWinCount += 1;

        CheckGameOver(stageData);
    }

    public void OnEnemyKillRequested()
    {
        if (File.Exists(filePath) == false) return; // 파일이 없다면 생생해준다.

        StageData stageData = ReturnStageData();
        stageData.PlayerWinCount += 1;

        CheckGameOver(stageData);
    }
}
