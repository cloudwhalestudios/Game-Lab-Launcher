using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ReactionSpeedMenu : MonoBehaviour
{
    public RectTransform optionContainer;
    public GameObject menuOptionPrefab;

    [Header("Colors", order = 1)]
    public Color selectedColor;
    public Color nextColor;
    public Color defaultColor;

    int currentOptionIndex;
    List<Image> optionImages;

    public void ShowMenu(List<float> reactionSpeedPresets, bool allowManual, char unitName, UnityAction<int> callback)
    {
        var maxOptionIndex = reactionSpeedPresets.Count + (allowManual ? 1 : 0);

        if (optionImages == null || optionImages?.Count <= 0)
        {
            optionImages = new List<Image>();

            for (int i = 0; i < maxOptionIndex; i++)
            {
                var newOption = Instantiate(menuOptionPrefab, optionContainer);
                int _i = i;
                //Debug.Log("Adding menu option " + _i);
                newOption.GetComponent<Button>().onClick.AddListener(() => callback(_i));
                newOption.name = "Option";
                if (allowManual && i == maxOptionIndex - 1)
                {
                    newOption.name += "Manual";
                    newOption.GetComponentInChildren<TextMeshProUGUI>().text = "Manual";
                }
                else
                {
                    newOption.name += reactionSpeedPresets[i] + unitName.ToString();
                    newOption.GetComponentInChildren<TextMeshProUGUI>().text = reactionSpeedPresets[i] + unitName.ToString();
                }

                optionImages.Add(newOption.GetComponent<Image>());
            }
        }
        UpdateSelectionDisplay(0);

        gameObject.SetActive(true);
    }

    public void UpdateSelectionDisplay(int optionIndex)
    {
        for (int i = 0; i < optionImages.Count; i++)
        {
            var option = optionImages[i];
            if (i == optionIndex)
            {
                option.color = selectedColor;
            }
            else if (i == (optionIndex + 1) % optionImages.Count)
            {
                option.color = nextColor;
            }
            else
            {
                option.color = defaultColor;
            }
        }
    }
}
