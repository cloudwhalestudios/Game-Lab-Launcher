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
    public VideoClip tutClip;
    public string tutUrl;

    public bool playVideoOnLoad = true;

    private void OnValidate()
    {
        if (validateTest)
        {
            validateTest = false;

            var screen = GetComponent<GameInfoScreen>();
            if (tutClip != null) screen.ShowInfo(devTitle, gameTitle, gameCover, tutClip, playVideoOnLoad);
            else screen.ShowInfo(devTitle, gameTitle, gameCover, tutUrl, playVideoOnLoad);
        }
    }
}
