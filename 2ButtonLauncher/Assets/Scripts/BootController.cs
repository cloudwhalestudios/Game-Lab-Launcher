using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootController : MonoBehaviour
{
    public float minBootDelay = 2f;
    public bool forceFullScreen = true;

    [Space]
    public string mainSceneName;
    public string setupSceneName;

    bool userIsSetup = false;

    private void Awake()
    {
        // TODO Add loading behaviour
        userIsSetup = false;
        Screen.fullScreen = forceFullScreen;
    }

    private void Start()
    {
        StartCoroutine(StartNextScene());
    }

    IEnumerator StartNextScene()
    {
        yield return new WaitForSecondsRealtime(minBootDelay);

        if (userIsSetup)
        {
            SceneManager.LoadScene(mainSceneName);
        }
        else
        {
            SceneManager.LoadScene(setupSceneName);
        }
        yield break;
    }
}
