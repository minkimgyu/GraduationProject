using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullSoundPlayer : ISoundPlayable
{
    public void Initialize(Dictionary<ISoundPlayable.SoundName, AudioClip> clipDictionary) { }

    public void PlayBGM(ISoundPlayable.SoundName name, float volumn = 1) { }

    public void PlaySFX(ISoundPlayable.SoundName name, float volumn = 1) { }
    public void PlaySFX(ISoundPlayable.SoundName name, Vector3 pos, float volumn = 1) { }

    public void StopAllSound() { }
    public void StopBGM() { }
}