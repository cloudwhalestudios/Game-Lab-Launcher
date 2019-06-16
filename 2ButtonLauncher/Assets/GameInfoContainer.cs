using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfoContainer : MonoBehaviour
{
    public GameInfo game;

    internal void SetInfo(GameInfo gameInfo)
    {
        game = gameInfo;
    }
}
