using AccessibilityInputSystem;
using PlayerPreferences;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebGLIntegration;
using static MainSceneController;

public class PlatformManager : MonoBehaviour
{
    public static event Action<PlatformState> PlatformStateChanged;

    public static PlatformManager Instance { get; private set; }
    public PlatformState CurrentState
    {
        get => currentState;
        private set
        {
            currentState = value;
            PlatformStateChanged?.Invoke(currentState);
        }
    }

    public enum PlatformState
    {
        Boot,
        Setup,
        Main
    }

    [Serializable]
    public enum GameName
    {
        None,
        JumpAndShoot,
        Wave,
        TwentyFourtyEight
    }

    [Header("Scene Control")]
    public string bootSceneName;
    public string setupSceneName;
    public string reactionSceneName;
    public string librarySceneName;
    public string exitSceneName;

    [SerializeField, ReadOnly] private PlatformState currentState;
    [SerializeField, ReadOnly] private string lastSceneName = "";
    [SerializeField, ReadOnly] private string currentSceneName = "";
    [SerializeField, ReadOnly] public bool canReturn = false;

    protected void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);

            UpdatePlatformState(SceneManager.GetActiveScene().name);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    protected void OnDestroy()
    {
        if (Instance == this) { Instance = null; }
        StopAllCoroutines();
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene oldScene, Scene newScene)
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.GameSelected);

        lastSceneName = currentSceneName;
        currentSceneName = newScene.name;
        UpdatePlatformState(newScene.name);
    }

    private void UpdatePlatformState(string sceneName)
    {
        if (sceneName.Equals(bootSceneName))
        {
            CurrentState = PlatformState.Boot;
        }
        else if (sceneName.Equals(setupSceneName) || sceneName.Equals(reactionSceneName))
        {
            CurrentState = PlatformState.Setup;
        }
        else if (sceneName.Equals(librarySceneName))
        {
            CurrentState = PlatformState.Main;
        }
    }

    public void ChangeScene(string sceneName)
    {
        if (sceneName != librarySceneName) canReturn = true;
        SceneManager.LoadScene(sceneName);
    }

    public void ReturnToLastScene()
    {
        if (canReturn)
        {
            canReturn = false;
            SceneManager.LoadScene(lastSceneName);
        }
        else
        {
            SceneManager.LoadScene(exitSceneName);
        }
    }

    public void LaunchGame(GameName name)
    {
        switch (name)
        {
            case GameName.JumpAndShoot:
                WebGLRedirect.OpenGame(Config.JUMP_SHOOT_GAME);
                break;
            case GameName.Wave:
                WebGLRedirect.OpenGame(Config.WAVE_GAME);
                break;
            case GameName.TwentyFourtyEight:
                WebGLRedirect.OpenGame(Config.NUMBERS_GAME);
                break;
            default:
                WebGLRedirect.OpenLauncher();
                break;
        }
    }

    public void Exit()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // For now it's just refresh
        WebGLRedirect.OpenLauncher();

#elif UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
