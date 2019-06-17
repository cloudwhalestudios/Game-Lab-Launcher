using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[Serializable, CreateAssetMenu(fileName = "Game", menuName = "Games/New Game")]
public class GameInfo : ScriptableObject
{
    public PlatformManager.GameName identifier;

    [Space]
    [TextArea] public string developer = "Cloudwhale";
    [TextArea] public string title = "Cloudwhale: A Game Placeholder";

    [Space]
    public Sprite cover;
    public VideoClip tutorial;
    public string tutorialUrl;

    public void SetInfo(GameInfo game)
    {
        this.identifier = game.identifier;
        this.developer = game.developer;
        this.title = game.title;
        this.cover = game.cover;
        this.tutorial = game.tutorial;
        this.tutorialUrl = game.tutorialUrl;
    }
}
