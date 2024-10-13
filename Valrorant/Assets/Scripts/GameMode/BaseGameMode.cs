using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseGameMode : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake() => Initialize();

    protected abstract void Initialize(); // 게임이 시작될  처리
    public abstract void OnGameClearRequested(); // 게임 클리어 시 처리
    public abstract void OnGameOverRequested(); // 게임 오버 시 처리
}