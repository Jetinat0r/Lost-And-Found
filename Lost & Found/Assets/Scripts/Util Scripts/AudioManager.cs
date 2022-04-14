using System;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public float soundFadeTime = 1f;

    public string songOnStart;

    public Sound[] sounds;
    [System.NonSerialized]
    public Sound curSong = null;

    //Use this for initialization
    private void Awake()
    {
        // Singleton Setup
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //Sound Player Setup
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.startVolume = s.volume;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        curSong = null;
    }

    private void Start()
    {
        PlaySong(songOnStart);
    }

    public void Play(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);

        if(s == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }

        if (s.isFading)
        {
            s.source.Stop();
            s.isFading = false;
            s.volume = s.startVolume;
        }

        s.source.Play();
    }

    //EventFinder Overload
    public void Play(EventFunctionParams functionParams)
    {
        Play(functionParams.stringParams[0]);
    }

    public void PlaySong(string songName, bool fadeOtherSong = true)
    {
        Sound s = Array.Find(sounds, sound => sound.name == songName);

        if(s == null)
        {
            Debug.LogWarning("Song: " + songName + " not found!");
            return;
        }

        if(curSong != null && !curSong.isFading)
        {
            StopSong(fadeOtherSong);
        }

        curSong = s;
        if (curSong.isFading)
        {
            if (curSong.source.isPlaying)
            {
                curSong.source.Stop();
            }
            curSong.isFading = false;
            curSong.volume = s.startVolume;
        }
        s.source.Play();
    }

    //EventFinder Overload
    public void PlaySong(EventFunctionParams functionParams)
    {
        PlaySong(functionParams.stringParams[0], functionParams.boolParams[0]);
    }

    private IEnumerator FadeSound(Sound s)
    {
        float timer = 0;

        s.isFading = true;

        while(s.isFading && timer < soundFadeTime)
        {
            s.volume = Mathf.Lerp(s.startVolume, 0, (timer / soundFadeTime));

            timer += Time.deltaTime;

            yield return null;
        }

        if (s.source.isPlaying)
        {
            s.source.Stop();
        }
        s.isFading = false;
        s.volume = s.startVolume;
    }

    public void Stop(string soundName, bool fadeOut = false)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }

        if (fadeOut)
        {
            StartCoroutine(FadeSound(s));
        }
        else
        {
            s.source.Stop();
        }
    }

    //EventFinder Overload
    public void StopSound(EventFunctionParams functionParams)
    {
        Stop(functionParams.stringParams[0], functionParams.boolParams[0]);
    }

    public void StopAllSounds(bool fadeOut = false)
    {
        foreach(Sound s in sounds)
        {
            if (fadeOut)
            {
                StartCoroutine(FadeSound(s));
            }
            else
            {
                s.source.Stop();
                curSong = null;
            }
        }
    }

    //EventFinder Overload
    public void StopAllSounds(EventFunctionParams functionParams)
    {
        StopAllSounds(functionParams.boolParams[0]);
    }

    public void StopSong(bool fadeOut = true)
    {
        Debug.Log("Stopping nothing?");
        //Debug.Log(curSong.name);
        //Debug.Log(curSong == null);
        if (curSong != null)
        {
            //Debug.Log(curSong.name);
            if (fadeOut)
            {
                StartCoroutine(FadeSound(curSong));
            }
            else
            {
                curSong.source.Stop();
            }
            curSong = null;
        }
        else
        {
            Debug.LogWarning("Can't stop song as there's no song currently playing!");
        }
    }

    //EventFinder Overload
    public void StopSong(EventFunctionParams functionParams)
    {
        StopSong(functionParams.boolParams[0]);
    }
}
