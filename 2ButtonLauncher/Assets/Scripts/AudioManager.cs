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
    public float lowestVolumeLevel = -80f;
    [Range(-80f, 20f)]
    public float highestVolumeLevel = 20f;

    [Space]
    public float muteVolume = 0f;
    public float lowVolume = 0.4f;
    public float normalVolume = 0.8f;
    public float highVolume = 1f;

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
            mixer.SetFloat("MasterVolume", PlatformPreferences.Current.PlatformVolumeLevel);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void Start()
    {
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

    }

    public void MuteAudio()
    {

    }

    void SetAudioLevel(float volumePercentage)
    {
        
    }
}
