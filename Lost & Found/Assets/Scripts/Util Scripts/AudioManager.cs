using System;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] sounds;
    public Sound curSong;

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
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        Play("Main Theme");
    }

    public void Play(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);

        if(s == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }

        s.source.Play();
    }

    public void PlaySong(string songName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == songName);

        if(s == null)
        {
            Debug.LogWarning("Song: " + songName + " not found!");
            return;
        }

        if(curSong != null)
        {
            curSong.source.Stop();
            curSong = null;
        }

        curSong = s;
        s.source.Play();
    }

    public void Play(QuestScriptableObject.FunctionParams functionParams)
    {
        Play(functionParams.stringParams[0]);
    }

    public void PlaySong(QuestScriptableObject.FunctionParams functionParams)
    {
        PlaySong(functionParams.stringParams[0]);
    }

    public void Stop(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }

        s.source.Stop();
    }
}
