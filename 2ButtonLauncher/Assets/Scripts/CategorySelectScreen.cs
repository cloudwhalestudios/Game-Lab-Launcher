using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CategorySelectScreen : MonoBehaviour
{
    public enum CategorySelectTransition
    {
        TopToBottom,
        BottomToTop
    }

    [Header("Category Area")]
    public RectTransform categoryContainer;
    public GameObject categoryPrefab;
    public List<GameObject> listedCategories;

    [Header("Layout")]
    public VerticalLayoutGroup layout;
    public float categoryRowHeight;

    [Header("Transition")]
    public CategorySelectTransition transition;
    [ColorUsage(true)] public Color activeTextColor;
    [ColorUsage(true)] public Color inactiveTextColor;

    Vector3 startPosition;
    int selectedIndex;

    private void Awake()
    {
        foreach (Transform child in categoryContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ShowCategories(int selection)
    {
        ShowCategories(null, -1, null, selection);
    }

    public void ShowCategories(List<GameCategory> categories, int loopCount, UnityAction<int, GameCategory> callback, int startSelection = -1)
    {
        selectedIndex = startSelection < 0 ? 0 : startSelection;

        if (listedCategories == null || listedCategories.Count == 0)
        {
            listedCategories = new List<GameObject>();

            for (int i = 0; i < categories.Count * loopCount; i++)
            {
                var category = categories[i % categories.Count];
                var _i = i;
                var listedCategory = Instantiate(categoryPrefab, categoryContainer);

                var container = listedCategory.GetComponent<CategoryContainer>();
                container.SetCategory(category);

                listedCategory.GetComponent<Button>().onClick.AddListener(() => callback(_i, category));
                listedCategories.Add(listedCategory);
            }

            SetupLayout(categories.Count * loopCount);
        }
        else
        {
            UpdateDisplay();
        }
        gameObject.SetActive(true);
    }

    private void UpdateDisplay()
    {
        UpdatePosition();
        for (int i = 0; i < listedCategories.Count; i++)
        {
            var categoryText = listedCategories[i].GetComponent<CategoryContainer>().textTitle;
            if (i == selectedIndex)
            {
                categoryText.color = activeTextColor;
            }
            else
            {
                categoryText.color = inactiveTextColor;
            }
        }
    }

    internal string GetName(int listingIndex = -1)
    {
        listingIndex = listingIndex < 0 ? selectedIndex : listingIndex;
        if (listingIndex < 0 || listingIndex > listedCategories.Count) return "";
        return listedCategories[listingIndex].GetComponent<CategoryContainer>().category.title;
    }

    public void HideCategories(bool cleanup = false)
    {
        if (cleanup)
        {
            for (int i = 0; i < listedCategories.Count; i++)
            {
                DestroyImmediate(listedCategories[i]);
            }
            listedCategories = null;
        }
        gameObject.SetActive(false);
    }

    void SetupLayout(int totalElements)
    {
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.padding.top = 0;
        layout.padding.bottom = 0;

        var totalHeight = totalElements * layout.spacing + (totalElements - 1) * categoryRowHeight;
        categoryContainer.sizeDelta = new Vector2(categoryContainer.sizeDelta.x, totalHeight);

        UpdateDisplay();
    }

    void UpdatePosition()
    {
        var offset = categoryRowHeight / 2f;

        var yPos = categoryContainer.sizeDelta.y / 2 - offset;
        yPos -= selectedIndex * (categoryRowHeight + layout.spacing);
        yPos = transition == CategorySelectTransition.BottomToTop ? yPos : -yPos;

        startPosition = new Vector3(0, yPos, 0);

        categoryContainer.localPosition = startPosition;
    }

    public void SelectNextCategory()
    {
        selectedIndex = (selectedIndex + 1) % listedCategories.Count;
        UpdateDisplay();
    }

    public void SelectPreviousCategory()
    {
        selectedIndex = (selectedIndex - 1 + listedCategories.Count) % listedCategories.Count;
        UpdateDisplay();
    }

    public void ExpandSelectedCategory()
    {
        listedCategories[selectedIndex]?.GetComponent<Button>()?.onClick?.Invoke();
    }
}
