using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupMenu : MonoBehaviour
{
    public RectTransform optionContainer;

    [Header("Colors", order = 1)]
    public Color selectedColor;
    public Color nextColor;
    public Color defaultColor;

    int currentOptionIndex;
    List<GameObject> options;

    public int CurrentOptionIndex { get => currentOptionIndex; set => currentOptionIndex = value; }
    public List<GameObject> Options { get => options; private set => options = value; }

    public virtual void ShowMenu(bool show = true, int defaultSelection = -1)
    {
        if (show)
        {
            Options = new List<GameObject>();
            var btns = optionContainer.GetComponentsInChildren<Button>();
            foreach (var btn in btns)
            {
                Options.Add(btn.gameObject);
            }
            CurrentOptionIndex = defaultSelection < 0 ? 0 : defaultSelection;

            UpdateSelectionDisplay();

            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void SelectNextOption ()
    {
        CurrentOptionIndex = (CurrentOptionIndex + 1) % Options.Count;
        UpdateSelectionDisplay();
    }

    public void SelectPreviousOption()
    {
        CurrentOptionIndex = (CurrentOptionIndex - 1 + Options.Count) % Options.Count;
        UpdateSelectionDisplay();
    }

    public void UseSelectedOption()
    {
        Options[CurrentOptionIndex]?.GetComponent<Button>()?.onClick?.Invoke();
    }

    public void UpdateSelectionDisplay(int optionIndex = -1)
    {
        CurrentOptionIndex = optionIndex < 0 ? CurrentOptionIndex : optionIndex;
        for (int i = 0; i < Options.Count; i++)
        {
            var optionImage = Options[i].GetComponent<Image>();
            if (i == currentOptionIndex)
            {
                optionImage.color = selectedColor;
            }
            else if (i == (currentOptionIndex + 1) % Options.Count)
            {
                optionImage.color = nextColor;
            }
            else
            {
                optionImage.color = defaultColor;
            }
        }
    }

    public Sprite GetIcon(int optionIndex = -1)
    {
        optionIndex = optionIndex < 0 ? CurrentOptionIndex : optionIndex;
        var img = Options[optionIndex]?.GetComponentInChildren<Image>();
        //Debug.Log("(" + Options[optionIndex].name + ": '" + img.overrideSprite + "') Getting sprite...");
        if (optionIndex < 0 || optionIndex > Options.Count) return null;
        return img.sprite;
    }

    public string GetText(int optionIndex = -1)
    {
        optionIndex = optionIndex < 0 ? CurrentOptionIndex : optionIndex;
        var tmpText = Options[optionIndex].GetComponentInChildren<TextMeshProUGUI>();

        //Debug.Log("(" + Options[optionIndex].name + ": '" + tmpText?.text + "') Getting text...");
        if (optionIndex < 0 || optionIndex > Options.Count) return "";
        return tmpText.text;
    }
}
