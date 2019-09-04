using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class BaseSetupController : MonoBehaviour
{
    [Serializable]
    protected class SetupStep
    {
        [Serializable]
        public struct TranslatedText
        {
            public string key;
            public int index;
            public float textDisplayTime;
        }

        public string title;
        public bool showTitle;

        [Header("Text")]
        public List<TranslatedText> textTranslationKeys;
    }

    [Header("References")]
    [SerializeField] protected TextMeshProUGUI tMPTitle;
    [SerializeField] protected TextMeshProUGUI tMPText;

    [Header("Setup Steps")]
    [SerializeField] protected List<SetupStep> setupSteps;

    //protected Coroutine previousSetupRoutine;
    protected Coroutine currentSetupRoutine;
    protected Coroutine currentUpdateTextRoutine;

    protected Coroutine StartNewCoroutine(IEnumerator newCoroutine, bool rememberLastRoutine = false)
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
        for (int i = 0; i < texts.Count; i++)
        {
            tMPText.text = LanguageManager.Instance.GetTranslation(texts[i].key, texts[i].index);
            yield return new WaitForSecondsRealtime(texts[i].textDisplayTime);
        }
    }
}
