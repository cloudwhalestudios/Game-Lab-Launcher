using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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
    public AudioSource Music;
    public bool playOnStart = false;

    [Header("SFX input")]
    public AudioSource Abort;
    public AudioSource Accept;
    public AudioSource GameSelected;
    public AudioSource Launch;
    public AudioSource Select;

    [Header("Sound effects")]
    public float lowPitchRange = 0.75f;
    public float highPitchRange = 1.25f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
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
            PlaySoundNormally(Music);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }


    public void PlaySound(AudioSource sound)
    {
        sound.pitch = Random.Range(lowPitchRange, highPitchRange);
        sound.Play(0);
    }

    public void PlaySoundNormally(AudioSource sound)
    {
        sound.Play(0);
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
