using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSelectScreen : MonoBehaviour
{
    [Header("Game Area")]
    public Transform gameContainer;
    public GameObject gamePrefab;
    public List<GameObject> listedGames;

    public void ShowGames(List<GameInfo> games, UnityAction callback)
    {
        listedGames = new List<GameObject>();

        foreach (var gameInfo in games)
        {
            var listedGame = Instantiate(gamePrefab, gameContainer);

            listedGame.GetComponent<GameInfo>().SetInfo(gameInfo);
            listedGame.GetComponent<Image>().sprite = gameInfo.cover;
            listedGame.GetComponent<Button>().onClick

            listedGames.Add(listedGame);
        }


    }

}
