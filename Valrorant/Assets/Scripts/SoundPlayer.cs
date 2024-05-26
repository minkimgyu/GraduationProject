using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    AudioSource _audioSource;

    private void Awake() => _audioSource = GetComponent<AudioSource>();

    public void Play(Vector3 pos, float ratio, Sound sound) // loop, transform도 추가해주자
    {
        _audioSource.volume = ratio;
        _audioSource.clip = sound.AudioClip;
        _audioSource.PlayOneShot(sound.AudioClip, 0.5f);

        transform.position = pos;
        Invoke("DisableObject", _audioSource.clip.length + 5.0f);
    }

    void DisableObject()
    {
        _audioSource.Stop();
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
        ObjectPooler.ReturnToPool(gameObject);
    }
}