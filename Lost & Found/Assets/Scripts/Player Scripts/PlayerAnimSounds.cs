using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimSounds : MonoBehaviour
{
    [SerializeField]
    private Sound[] pickupSounds;
    [SerializeField]
    private Sound[] walkSounds;

    private Sound curPickupSound;

    private void Awake()
    {
        if(pickupSounds.Length == 0)
        {
            Debug.LogWarning("No sounds found in PlayerAnimSounds for \"Pickup Sounds\", will cause errors!");
        }

        foreach (Sound s in pickupSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.startVolume = s.volume;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }


        if (walkSounds.Length == 0)
        {
            Debug.LogWarning("No sounds found in PlayerAnimSounds for \"Walk Sounds\", will cause errors!");
        }

        foreach (Sound s in walkSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.startVolume = s.volume;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void PlayPickupSound()
    {
        if (curPickupSound != null && curPickupSound.source.isPlaying)
        {
            curPickupSound.source.Stop();
            curPickupSound = null;
        }

        curPickupSound = pickupSounds[Random.Range(0, pickupSounds.Length)];
        curPickupSound.source.Play();
    }

    public void PlayWalkSound()
    {
        Sound nextSound = walkSounds[Random.Range(0, walkSounds.Length)];

        if (nextSound.source.isPlaying)
        {
            nextSound.source.Stop();
        }

        nextSound.source.Play();
    }
}
