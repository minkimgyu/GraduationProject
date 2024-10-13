using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour, ISoundPlayable
{
    Dictionary<ISoundPlayable.SoundName, AudioClip> _clipDictionary;

    AudioSource _bgmPlayer;
    AudioSource[] _sfxPlayer;

    [SerializeField] GameObject _bgmPlayerObject;
    [SerializeField] GameObject _sfxPlayerObject;

    public void Initialize(Dictionary<ISoundPlayable.SoundName, AudioClip> clipDictionary)
    {
        _clipDictionary = clipDictionary;
        _bgmPlayer = _bgmPlayerObject.GetComponent<AudioSource>();
        _bgmPlayer.loop = true;

        _sfxPlayer = _sfxPlayerObject.GetComponents<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    public void PlayBGM(ISoundPlayable.SoundName name, float volumn = 1)
    {
        _bgmPlayer.clip = _clipDictionary[name];

        _bgmPlayer.volume = volumn;
        _bgmPlayer.Play();
    }

    public void PlaySFX(ISoundPlayable.SoundName name, Vector3 pos, float volumn = 1)
    {
        AudioSource.PlayClipAtPoint(_clipDictionary[name], pos, volumn); // --> Factory 패턴 사용
    }

    public void PlaySFX(ISoundPlayable.SoundName name, float volumn = 1)
    {
        for (int i = 0; i < _sfxPlayer.Length; i++)
        {
            if (_sfxPlayer[i].isPlaying == true) continue;

            _sfxPlayer[i].clip = _clipDictionary[name];

            _sfxPlayer[i].volume = volumn;
            _sfxPlayer[i].Play();
            break;
        }
    }

    public void StopBGM()
    {
        _bgmPlayer.Stop();
    }

    public void StopAllSound()
    {
        _bgmPlayer.Stop();
        for (int i = 0; i < _sfxPlayer.Length; i++)
        {
            _sfxPlayer[i].Stop();
        }
    }
}