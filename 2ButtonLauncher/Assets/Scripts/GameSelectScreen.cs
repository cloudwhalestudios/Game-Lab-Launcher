using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameSelectScreen : MonoBehaviour
{
    public enum GameSelectTransition
    {
        LeftToRight,
        RightToLeft
    }

    [Header("Game Area")]
    public RectTransform gameContainer;
    public GameObject gamePrefab;
    public List<GameObject> listedGames;

    [Header("Layout")]
    public HorizontalLayoutGroup layout;
    public float gameCoverSize;

    [Header("Transition")]
    public GameSelectTransition transition;

    Vector3 startPosition;
    int selectedIndex;

    private void Awake()
    {
        foreach (Transform child in gameContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ShowGames(int selection)
    {
        ShowGames(null, -1, null, selection);
    }

    public void ShowGames(List<GameInfo> games, int loopCount, UnityAction<int, GameInfo> callback, int startSelection = -1)
    {
        selectedIndex = startSelection < 0 ? 0 : startSelection;

        if (listedGames == null || listedGames.Count == 0)
        {
            listedGames = new List<GameObject>();

            for (int i = 0; i < games.Count * loopCount; i++)
            {
                var gameInfo = games[i % games.Count];
                var _i = i;
                var listedGame = Instantiate(gamePrefab, gameContainer);

                listedGame.GetComponent<GameInfoContainer>().SetInfo(gameInfo);
                listedGame.GetComponent<Image>().sprite = gameInfo.cover;

                listedGame.GetComponent<Button>().onClick.AddListener(() => callback(_i, gameInfo));

                listedGames.Add(listedGame);
            }

            SetupLayout(games.Count * loopCount);

        }
        else
        {
            UpdatePosition();
        }
        gameObject.SetActive(true);
    }

    internal string GetName(int listingIndex = -1)
    {
        listingIndex = listingIndex < 0 ? selectedIndex : listingIndex;
        if (listingIndex < 0 || listingIndex > listedGames.Count) return "";
        return listedGames[listingIndex].GetComponent<GameInfoContainer>().game.title;
    }

    public void HideGames(bool cleanup = false)
    {
        if (cleanup)
        {
            for (int i = 0; i < listedGames.Count; i++)
            {
                DestroyImmediate(listedGames[i]);
            }
            listedGames = null;
        }
        gameObject.SetActive(false);
    }

    void SetupLayout(int totalElements)
    {
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.padding.right = 0;
        layout.padding.left = 0;

        var totalWidth = totalElements * layout.spacing + (totalElements-1) * gameCoverSize;
        gameContainer.sizeDelta = new Vector2(totalWidth, gameContainer.sizeDelta.y);

        UpdatePosition();
    }
    
    void UpdatePosition()
    {
        var offset = gameCoverSize / 2f;

        var xPos = gameContainer.sizeDelta.x / 2 - offset;
        xPos -= selectedIndex * (gameCoverSize + layout.spacing);
        xPos = transition == GameSelectTransition.RightToLeft ? -xPos : xPos;

        startPosition = new Vector3(xPos, 0, 0);

        gameContainer.localPosition = startPosition;
    }

    public void SelectNextGame()
    {
        selectedIndex = (selectedIndex + 1) % listedGames.Count;
        UpdatePosition();
    }

    public void SelectPreviousGame()
    {
        selectedIndex = (selectedIndex - 1 + listedGames.Count) % listedGames.Count;
        UpdatePosition();
    }

    public void UseSelectedGame()
    {
        listedGames[selectedIndex]?.GetComponent<Button>()?.onClick?.Invoke();
    }
}
