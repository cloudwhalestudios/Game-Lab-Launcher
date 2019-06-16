using AccessibilityInputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibraryController : MonoBehaviour
{
    public enum ScreenState
    {
        CategorySelect,
        SubCategorySelect,
        GameSelect,
        GameInfo
    }

    [SerializeField, ReadOnly] private ScreenState lastState;
    [SerializeField, ReadOnly] private ScreenState currentState;

    public ScreenState CurrentState
    {
        get { return currentState; }
        private set
        {
            currentState = value;
        }
    }

    public void ReturnToLastScreen()
    {

    }

    public void QuitLauncher()
    {
        // TODO add timeout prompt asking if they want to quit
        PlatformManager.Instance.ChangeScene(PlatformManager.Instance.exitSceneName);
    }

    public void OpenHomeScreen()
    {

    }
}
