using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class GameInfo : MonoBehaviour
{
    [TextArea] public string developer = "Cloudwhale";
    [TextArea] public string title = "Cloudwhale: A Game Placeholder";

    public Sprite cover;
    public VideoClip tutorial;
    public string tutorialUrl;

    public void SetInfo(GameInfo game)
    {
        this.developer = game.developer;
        this.title = game.title;
        this.cover = game.cover;
        this.tutorial = game.tutorial;
        this.tutorialUrl = game.tutorialUrl;
    }
}
