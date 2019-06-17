using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "Game", menuName = "Games/New Category", order = 0)]
public class GameCategory : ScriptableObject
{
    [TextArea] public string title;

    public List<GameInfo> games;
}
