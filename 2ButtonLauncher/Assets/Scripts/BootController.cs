using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootController : MonoBehaviour
{
    [Header("Boot Options")]
    [SerializeField] private float minBootDelay = 2f;
    [SerializeField] private RectTransform timeIndicator;

    [SerializeField] private bool forceFullScreen = false;
    [SerializeField] private bool resetPlayerPrefs = false;

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
        AudioManager.Instance.PlaySound(AudioManager.Instance.Launch);
        StartCoroutine(StartNextScene());

        // Load the platform preferences
        userIsSetup = PlatformPreferences.Current.CompletedSetup;
        
        finishedLoading = true;
    }

    IEnumerator StartNextScene()
    {
        var elapsedTime = 0f;

        while (true)
        {
            if (timeIndicator != null)
            {
                if (elapsedTime >= minBootDelay) break;

                var percentage = Mathf.Clamp01(elapsedTime / minBootDelay);
                timeIndicator.localScale = new Vector3(1 - percentage, 1, 1);
                yield return new WaitForEndOfFrame();
                elapsedTime += Time.deltaTime;
            }
            else
            {
                yield return new WaitForSeconds(minBootDelay);
                break;
            }
        }

        timeIndicator.localScale = new Vector3(0, 1, 1);

        if (!finishedLoading)
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
            SceneManager.LoadScene(PlatformManager.Instance.librarySceneName);
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
