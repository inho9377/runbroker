using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using Logic;

public class SoundManager : CSingletonMonobehaviour<SoundManager> {

    public AudioSource audioSource;
    
    public AudioClip unityChanSound;
    public AudioClip knightSound;
    public AudioClip toonbotSound;
    public AudioClip tededySound;

    
    void Start ()
    {
        DontDestroyOnLoad(this);
    }
	

    public void PlaySfx(AudioClip clip, bool isLoop = false, bool isStop = false)
    {
        if (isStop)
            Stop();

        audioSource.loop = isLoop;
        audioSource.PlayOneShot(clip);
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    public void Pause()
    {
        audioSource.Pause();
    }

    public void PlayCharSound(Common.PlayerType type)
    {
        switch(type)
        {
            case Common.PlayerType.KNIGHT:
                audioSource.PlayOneShot(knightSound);
                break;
            case Common.PlayerType.TEDDY:
                audioSource.PlayOneShot(tededySound);
                break;
            case Common.PlayerType.TOON_BOT:
                audioSource.PlayOneShot(toonbotSound);
                break;
            case Common.PlayerType.UNITY_CHAN:
                audioSource.PlayOneShot(unityChanSound);
                break;
        }
    }

}
