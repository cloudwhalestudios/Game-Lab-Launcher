using AccessibilityInputSystem.TwoButtons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneController : MonoBehaviour
{
    public BaseStateMenuController stateMenuController;

    private void OnEnable()
    {
        PlatformPlayer.MainPrimary += PlatformPlayer_MainPrimary;
        PlatformPlayer.MainSecondary += PlatformPlayer_MainSecondary;
    }

    private void OnDisable()
    {
        PlatformPlayer.MainPrimary += PlatformPlayer_MainPrimary;
        PlatformPlayer.MainSecondary += PlatformPlayer_MainSecondary;
    }

    private void PlatformPlayer_MainPrimary()
    {
        StateMenuManager.Instance.Select();
    }

    private void PlatformPlayer_MainSecondary()
    {
        StateMenuManager.Instance.Return();
    }

    private void Start()
    {
        StateMenuManager.Instance.SetStateMenuController(stateMenuController);
        StateMenuManager.Instance.StartIndicating();
    }
}
