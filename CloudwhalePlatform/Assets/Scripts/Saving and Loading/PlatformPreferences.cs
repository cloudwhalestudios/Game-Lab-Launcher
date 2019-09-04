using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlatformPreferences
{
    static PlatformPreferences current;

    [SerializeField] bool completedSetup;
    [SerializeField] string language;
    [SerializeField] KeyCode[] keys;
    [SerializeField] float menuProgressionTimer = 5f;
    [SerializeField] float platformVolumeLevel = -1f;
    [SerializeField] bool platformMute = false;

    [SerializeField] float gameVolumeLevel = -1f;
    [SerializeField] bool gameMute = false;

    public static PlatformPreferences Current
    {
        get
        {
            if (current != null)
                return current;

            current = PreferenceManager.Load<PlatformPreferences>();

            return current;
        }
    }

    public bool CompletedSetup { get => completedSetup; set { completedSetup = value; Save(); } }
    public KeyCode[] Keys { get => keys; set { keys = value; Save(); } }
    public float ReactionTime { get => menuProgressionTimer; set { menuProgressionTimer = value; Save(); } }
    public float PlatformVolumeLevel { get => platformVolumeLevel; set { platformVolumeLevel = value; Save(); } }
    public float GameVolumeLevel { get => gameVolumeLevel; set { gameVolumeLevel = value; Save(); } }
    public string Language { get => language; set { language = value; Save(); } }
    public bool PlatformMute { get => platformMute; set { platformMute = value; Save(); } }
    public bool GameMute { get => gameMute; set { gameMute = value; Save(); } }


    public static void Save()
    {
        PreferenceManager.Save(current);
    }
}
