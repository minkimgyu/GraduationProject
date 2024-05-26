using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public Sound(string name, AudioClip audioClip)
    {
        this.name = name;
        this.audioClip = audioClip;
    }

    [SerializeField]
    string name;
    public string Name { get { return name; } }

    [SerializeField]
    AudioClip audioClip;
    public AudioClip AudioClip { get { return audioClip; } }
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager _instance;

    [SerializeField]
    List<Sound> _sounds;

    AudioSource _bgmPlayer;

    [SerializeField] float _sfxMasterVolume = 1;
    [SerializeField] float _bgmMasterVolume = 1;

    private void Awake()
    {
        _instance = this;
        _bgmPlayer = GetComponent<AudioSource>();
    }

    public static void StopBGM()
    {
        if (_instance._bgmPlayer.isPlaying == true) _instance._bgmPlayer.Stop();
    }

    public static void PlayBGM(string name)
    {
        Sound sound = _instance._sounds.Find(x => x.Name == name);
        if (sound == null) return;

        StopBGM();

        _instance._bgmPlayer.clip = sound.AudioClip;
        _instance._bgmPlayer.Play();
    }

    public static void PlaySFX(Vector3 pos, string name, float sfxVolume = 1)
    {
        SoundPlayer player = ObjectPooler.SpawnFromPool<SoundPlayer>("SfxPlayer");
        if (player == null) return;

        Sound sound = _instance._sounds.Find(x => x.Name == name);
        if (sound == null) return;

        player.Play(pos, sfxVolume * _instance._sfxMasterVolume, sound);
    }
}