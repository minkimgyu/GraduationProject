using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocater
{
    static ISoundPlayable _soundPlayer;
    static NullSoundPlayer _nullSoundPlayer;

    static ISceneControllable _sceneController;
    static NullSceneController _nullSceneController;

    static ServiceLocater()
    {
        _nullSoundPlayer = new NullSoundPlayer();
        _nullSceneController = new NullSceneController();
    }

    public static void Provide(ISoundPlayable soundPlayer)
    {
        _soundPlayer = soundPlayer;
    }

    public static void Provide(ISceneControllable sceneController)
    {
        _sceneController = sceneController;
    }


    public static ISoundPlayable ReturnSoundPlayer()
    {
        if (_soundPlayer == null) return _nullSoundPlayer;
        return _soundPlayer;
    }

    public static ISceneControllable ReturnSceneController()
    {
        if (_sceneController == null) return _nullSceneController;
        return _sceneController;
    }
}