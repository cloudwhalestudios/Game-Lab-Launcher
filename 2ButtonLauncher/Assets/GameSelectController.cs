using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSelectController : MonoBehaviour
{
    public InputBarButtonState selectControllerButtonState;
    public GameSelectScreen screen;

    InputBarController barController;

    private void Awake()
    {
        barController = GameObject.FindGameObjectWithTag("InputBar").GetComponent<InputBarController>();
        //infoControllerButtonState.SetActive();
    }

    public void OpenGameSelectScreen(List<GameInfo> games)
    {

    }
}
