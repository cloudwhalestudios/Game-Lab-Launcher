using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootController : BaseController
{
    [Header("Boot Options")]
    [SerializeField] private float minBootDelay = 2f;
    [SerializeField] private RectTransform timeIndicator;

    bool isUserSetup = false;
    bool finishedLoading = false;
    bool interrupt = false;

    public override void StartController()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.Launch);
        StartCoroutine(LoadCoroutine());
        StartCoroutine(BootCoroutine());
    }

    private IEnumerator LoadCoroutine()
    {
        // Load the platform preferences a
        isUserSetup = PlatformPreferences.Current.CompletedSetup;
        yield return LanguageManager.Instance.FetchTranslations(PlatformPreferences.Current.Language);

        finishedLoading = true;
    }

    IEnumerator BootCoroutine()
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
            isUserSetup = PlatformPreferences.Current.CompletedSetup = false;
            PlayerPrefs.DeleteAll();
        }

        ScenePartsManager.Instance.FinishBooting(isUserSetup);
    }

    public void ClearPlayerPrefs()
    {
        Debug.Log("Clearing Player Preferences");
        interrupt = true;
    }
}
