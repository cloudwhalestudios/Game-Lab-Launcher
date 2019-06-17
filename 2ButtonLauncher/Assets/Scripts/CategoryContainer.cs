using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryContainer : MonoBehaviour
{
    public RectTransform previewContainer;
    public TextMeshProUGUI textTitle;
    public GameObject previewPrefab;
    public int minPreviews = 5;

    [Space]
    public GameCategory category;

    [Space]
    public List<GameObject> gamePreviews;

    public void SetCategory(GameCategory gameCategory)
    {
        category = gameCategory;
        textTitle.text = category.title;
        CreatePreviews();
    }

    void CreatePreviews()
    {
        var games = category.games;
        foreach (Transform child in previewContainer.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < games.Count * minPreviews; i++)
        {
            var gameInfo = games[i%games.Count];
            var preview = Instantiate(previewPrefab, previewContainer);
            preview.name = "GamePreview" + gameInfo.title;
            preview.GetComponent<Image>().sprite = gameInfo.cover;
            gamePreviews.Add(preview);
        }
    }
}
