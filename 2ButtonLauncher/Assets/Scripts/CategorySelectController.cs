﻿using AccessibilityInputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategorySelectController : MonoBehaviour
{
    public InputBarButtonState categoryControllerButtonState;
    public CategorySelectScreen screen;
    public LauncherOptionsController launcherOptionsController;

    [Space]
    public string selectionPrefix = "View";

    [Space]
    public List<GameCategory> testCategories;

    [SerializeField, ReadOnly] int lastIndex;
    InputBarController barController;
    int baseLoopCount;

    public bool IsOpen => screen.gameObject.activeInHierarchy && categoryControllerButtonState.isActiveAndEnabled;

    private void Start()
    {
        barController = GameObject.FindGameObjectWithTag("InputBar").GetComponent<InputBarController>();
        launcherOptionsController.Close();
        if (testCategories != null && testCategories.Count > 0)
        {
            OpenCategorySelectScreen(testCategories);
        }
    }

    private void OnEnable()
    {
        InputBarController.TimerElapsed += InputBarController_TimerElapsed;
    }


    private void OnDisable()
    {
        InputBarController.TimerElapsed -= InputBarController_TimerElapsed;
    }

    private void InputBarController_TimerElapsed()
    {
        if (IsOpen)
        {
            screen.SelectNextCategory();
            categoryControllerButtonState.ChangeCurrentButtonDisplay(selectionPrefix + " " + screen.GetName(), null);
        }
    }

    public void ReopenCategorySelectScreen()
    {
        screen.ShowCategories(lastIndex);

        baseLoopCount = categoryControllerButtonState.LoopCount;
        categoryControllerButtonState.LoopCount = screen.listedCategories.Count * baseLoopCount;
        categoryControllerButtonState.SetActive();

        categoryControllerButtonState.ChangeCurrentButtonDisplay(selectionPrefix + " " + screen.GetName(), null);
    }

    public void OpenCategorySelectScreen(List<GameCategory> categories)
    {
        lastIndex = 0;
        screen.ShowCategories(categories, categoryControllerButtonState.LoopCount, SelectCategory, lastIndex);

        baseLoopCount = categoryControllerButtonState.LoopCount;
        categoryControllerButtonState.LoopCount *= categories.Count;
        categoryControllerButtonState.SetActive();

        categoryControllerButtonState.ChangeCurrentButtonDisplay(selectionPrefix + " " + screen.GetName(), null);
    }

    public void CloseCategorySelectScreen(bool cleanupList = false)
    {

        if (cleanupList) lastIndex = 0;
        if (baseLoopCount > 0) categoryControllerButtonState.LoopCount = baseLoopCount;
        launcherOptionsController.Close();

        screen.HideCategories(cleanupList);
        categoryControllerButtonState.SetActive(false);
    }

    public void SelectCurrentCategory()
    {
        screen.ExpandSelectedCategory();
    }

    public void SelectCategory(int listingIndex, GameCategory category)
    {
        CloseCategorySelectScreen();
        lastIndex = listingIndex;
        categoryControllerButtonState.SetActive(false);
        LibraryController.Instance.ViewGameSelection(category);
    }

    public void ToggleLauncherOptionsPopup()
    {
        if (launcherOptionsController.IsOpen)
        {
            launcherOptionsController.Close();
            categoryControllerButtonState.SetActive();
        }
        else
        {
            launcherOptionsController.Open();
        }
    }
}
