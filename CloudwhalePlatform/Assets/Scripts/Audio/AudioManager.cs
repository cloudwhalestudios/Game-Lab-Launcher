﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Volume Control")]
    public AudioMixer mixer;
    [Range(-80f, 20f)]
    public float lowestVolume = -80f;
    [Range(-80f, 20f)]
    public float highestVolume = 20f;

    [Space]
    [Range(0, 1f)]
    public float defaultVolumeLevel = 0.5f;
    public float volumeStep = 0.25f;

    [Header("Music input")]
    public AudioSource musicSource;
    public bool playOnStart = false;

    [Header("SFX input")]
    public AudioSource sfxSource;
    public AudioClip Abort;
    public AudioClip Accept;
    public AudioClip GameSelected;
    public AudioClip Launch;
    public AudioClip Select;

    [Header("Sound effects")]
    public float lowPitchRange = 0.75f;
    public float highPitchRange = 1.25f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void Start()
    {
        if (PlatformPreferences.Current.PlatformVolumeLevel < 0)
        {
            PlatformPreferences.Current.PlatformVolumeLevel = defaultVolumeLevel;
        }
        SetVolumeLevel(PlatformPreferences.Current.PlatformVolumeLevel);
        if (playOnStart)
        {
            musicSource.Play();
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }


    public void PlaySound(AudioClip soundClip)
    {
        sfxSource.pitch = Random.Range(lowPitchRange, highPitchRange);
        sfxSource.PlayOneShot(soundClip);
    }

    public void PlaySoundNormally(AudioClip soundClip)
    {
        sfxSource.pitch = 1;
        sfxSource.PlayOneShot(soundClip);
    }

    public void UnmuteAudio()
    {
        PlatformPreferences.Current.PlatformMute = false;
        SetVolumeLevel(PlatformPreferences.Current.PlatformVolumeLevel);
    }

    public void MuteAudio()
    {
        PlatformPreferences.Current.PlatformMute = true;
        SetVolumeLevel(PlatformPreferences.Current.PlatformVolumeLevel);
    }

    public void DecreaseVolume()
    {
        SetVolumeLevel(PlatformPreferences.Current.PlatformVolumeLevel - volumeStep);
    }

    public void IncreaseVolume()
    {
        SetVolumeLevel(PlatformPreferences.Current.PlatformVolumeLevel + volumeStep);
    }

    void SetVolumeLevel(float volumeLevel)
    {
        volumeLevel = Mathf.Clamp01(volumeLevel);
        var volumeRange = highestVolume - lowestVolume;
        var volume = lowestVolume + volumeLevel * volumeRange;

        PlatformPreferences.Current.PlatformVolumeLevel = volumeLevel;

        if (PlatformPreferences.Current.PlatformMute)
        {
            volume = lowestVolume;
        }
        //Debug.Log("volume set to " + volume);
        mixer.SetFloat("MasterVolume", volume);
    }
}
