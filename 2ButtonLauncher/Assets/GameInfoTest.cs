using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameInfoTest : MonoBehaviour
{
    public bool validateTest = false;

    [TextArea] public string devTitle = "Cloudwhale";
    [TextArea] public string gameTitle = "Cloudwhale: A Game Placeholder";

    public Sprite gameCover;
    public VideoClip tutorialClip;

    public bool playVideoOnLoad = true;

    private void OnValidate()
    {
        if (validateTest)
        {
            validateTest = false;

            var screen = GetComponent<GameInfoScreen>();
            screen.SetDisplayInfo(devTitle, gameTitle, gameCover, tutorialClip, playVideoOnLoad);
        }
    }
}
