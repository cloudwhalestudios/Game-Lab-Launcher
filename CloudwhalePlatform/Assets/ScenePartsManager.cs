using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePartsManager : MonoBehaviour
{
    [Serializable]
    public class ScenePart
    {
        public enum Identifier
        {
            Boot,
            InputSetup,
            SpeedSetup,
            CategoryView,
            GamesView,
            InfoView,
            OptionMenu,
            Popup,
            InputBar
        }

        public enum Type
        {
            Main,
            Popup,
            Overlay
        }

        public string sceneAssetName;
        public Identifier indentifier;
        public Type type;

        public bool IsLoaded => reference != null && reference.isLoaded;
        public bool IsActive => reference == SceneManager.GetActiveScene();
        [ReadOnly] public Scene reference;
    }

    public enum State
    {
        Boot,
        Setup,
        Library,
        Popup
    }

    public static ScenePartsManager Instance { get; private set; }


    [ReadOnly, SerializeField] private State activeState;

    public ScenePart bootPart;
    public ScenePart inputBarPart;
    public ScenePart optionMenuPart;
    public ScenePart popupPart;

    public List<ScenePart> setupPartList;
    public List<ScenePart> libraryPartList;

    // Keeping track of active and visited parts for easy navigation and debugging
    // TODO: needs logging
    [ReadOnly, SerializeField] private ScenePart activeMainPart;
    [SerializeField] private Stack<ScenePart> previousMainParts;

    public State ActiveState { get => activeState; set => activeState = value; }

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
        // 1. (Editor) Close all open scenes
#if UNITY_EDITOR
        for (int i = 1; i < SceneManager.sceneCount; i++)
        {
            Scene loadedScene = SceneManager.GetSceneAt(i);
            SceneManager.UnloadSceneAsync(loadedScene);
        }
#endif

        // 2. Boot
        AddScenePart(bootPart);
    }

    public void FinishBooting(bool isUserSetup)
    {
        // Check user status
        //  a. if user is first time user goto setup
        if (!isUserSetup)
        {
            StartSetup();
        }
        //  b. if user is returning user and new setups were added inform user of news and prompt for setup
        else if (false /* isSetupUpdateAvailable */)
        {
            //  if user wants new setup goto new setup queue (skip old)
            StartSetup(1);
        }
        //  c. otherwise there is nothing to do so go straightto the library
        else
        {
            StartLibrary();
        }
    }

    private void StartSetup(int position = 0)
    {
        AddScenePart(setupPartList[position]);
    }

    private void StartLibrary()
    {
        AddScenePart(libraryPartList[0]);
    }

    private void AddScenePart(ScenePart part)
    {
        // Check if part is valid
        if (part == null) return;
        StartCoroutine(LoadScenePart(part));
    }

    private void RemoveScenePart(ScenePart part)
    {
        if (part == null) return;
        StartCoroutine(UnloadScenePart(part));
    }

    private IEnumerator LoadScenePart(ScenePart part)
    {
        // Remove main part if new one is to be added
        if (part.type == ScenePart.Type.Main && activeMainPart != null)
            yield return UnloadScenePart(activeMainPart);

        // Load part (add it to scene)
        if (!part.IsLoaded)
            yield return SceneManager.LoadSceneAsync(part.sceneAssetName, LoadSceneMode.Additive);
        
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(part.sceneAssetName));
        part.reference = SceneManager.GetActiveScene();
        
        if (part.type == ScenePart.Type.Main) activeMainPart = part;

        yield return new WaitForSeconds(2f);
    }

    private IEnumerator UnloadScenePart(ScenePart part)
    {
        if (!part.IsLoaded) yield break;
        Debug.Log("Unloading " + part.sceneAssetName);
        yield return SceneManager.UnloadSceneAsync(part.reference);
        yield return null;
    }
}
