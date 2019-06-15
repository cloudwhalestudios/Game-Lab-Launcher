using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
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
    public float volumeValue;

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

}
