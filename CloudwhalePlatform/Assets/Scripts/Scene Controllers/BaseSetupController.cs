using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class BaseSetupController : BaseController
{
    [Serializable]
    protected class SetupStep
    {
        [Serializable]
        public class TranslatedText
        {
            [Header("Language File Key/Index")]
            public string key;
            public int index;

            [Header("Display Settings")]
            public float displayTime;
            [Space]
            public bool overrideColor = false;
            public Color color;
        }

        [Header("Title")]
        public TranslatedText titleKey;
        public bool showTitle = true;

        [Header("Text")]
        public List<TranslatedText> textTranslationKeys;
    }

    [Header("References")]
    [SerializeField] protected TextMeshProUGUI tMPTitle;
    [SerializeField] protected TextMeshProUGUI tMPText;
    [SerializeField] protected Color defaultTextColor;


    [Header("Setup Steps")]
    [SerializeField] protected List<SetupStep> setupSteps;



    //protected Coroutine previousSetupRoutine;
    protected Coroutine currentSetupRoutine;
    protected Coroutine currentUpdateTextRoutine;
    protected Coroutine currentWarningRoutine;
    protected int currentStepIndex;

    protected Coroutine StartNewSetupCoroutine(IEnumerator newCoroutine, bool rememberLastRoutine = false)
    {
        //if (rememberLastRoutine) previousSetupRoutine = currentSetupRoutine;
        if (currentSetupRoutine != null) StopCoroutine(currentSetupRoutine);
        return currentSetupRoutine = StartCoroutine(newCoroutine);
    }

    protected Coroutine StartNewTextUpdateCoroutine(List<SetupStep.TranslatedText> texts)
    {
        if (currentUpdateTextRoutine != null) StopCoroutine(currentUpdateTextRoutine);
        return currentUpdateTextRoutine = StartCoroutine(UpdateTextCoroutine(texts));
    }

    protected IEnumerator UpdateTextCoroutine(List<SetupStep.TranslatedText> texts)
    {
        tMPText.gameObject.SetActive(true);
        for (int i = 0; i < texts.Count; i++)
        {
            UpdateText(tMPText, texts[i]);
            if (texts[i].displayTime <= 0) yield break;

            yield return new WaitForSecondsRealtime(texts[i].displayTime);
        }
        tMPText.gameObject.SetActive(false);
    }

    protected void UpdateText(TextMeshProUGUI target, SetupStep.TranslatedText text)
    {
        target.text = LanguageManager.Instance.GetTranslation(text.key, text.index);
        
        target.color = text.overrideColor ? text.color : defaultTextColor;
    }
    
    protected void StartNewWarningCoroutine(SetupStep.TranslatedText warning)
    {
        if (currentWarningRoutine != null) StopCoroutine(currentWarningRoutine);
        currentWarningRoutine = StartCoroutine(IssueWarningCoroutine(warning));
    }

    protected IEnumerator IssueWarningCoroutine(SetupStep.TranslatedText warning)
    {
        UpdateText(tMPTitle, warning);
        tMPText.gameObject.SetActive(false);

        if (warning.displayTime <= 0) yield break;
        yield return new WaitForSecondsRealtime(warning.displayTime);

        UpdateText(tMPTitle, setupSteps[currentStepIndex].titleKey);
        tMPText.gameObject.SetActive(true);
    }
}
