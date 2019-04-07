using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootController : MonoBehaviour
{
    [Header("Boot Options")]
    public float minBootDelay = 2f;
    public bool forceFullScreen = true;
    public bool resetPlayerPrefs = false;

    bool userIsSetup = false;
    bool finishedLoading = false;
    bool interrupt = false;

    private void Awake()
    {
        // TODO Add loading behaviour
        Screen.fullScreen = forceFullScreen;
    }

    private void Start()
    {
        StartCoroutine(StartNextScene());

        // Load the platform preferences
        userIsSetup = PlatformPreferences.Current.CompletedSetup;
        
        finishedLoading = true;
    }

    IEnumerator StartNextScene()
    {
        yield return new WaitForSecondsRealtime(minBootDelay);

        if(!finishedLoading)
        {
            yield return new WaitForEndOfFrame();
        }

        if (interrupt)
        {
            userIsSetup = false;
            PlayerPrefs.DeleteAll();
        }

        if (userIsSetup)
        {
            BootLoader.LoadPlatformPlayer();
            SceneManager.LoadScene(PlatformManager.Instance.mainSceneName);
        }
        else
        {
            SceneManager.LoadScene(PlatformManager.Instance.setupSceneName);
        }
        yield break;
    }

    public void ClearPlayerPrefs()
    {
        Debug.Log("Clearing Player Preferences");
        interrupt = true;
    }
}
