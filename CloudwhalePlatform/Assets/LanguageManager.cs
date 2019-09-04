using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance { get; private set; }
    public string fallbackLanguage = "Lang_EN";
    public List<string> availableLanguages = new List<string>()
    {
        "Lang_EN",
        "Lang_NL",
    };

    private JSONNode translationsJSON;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    public IEnumerator FetchTranslations(string selectedLanguage = "")
    {
        // TODO: figure out local language

        if (!availableLanguages.Contains(selectedLanguage)) selectedLanguage = fallbackLanguage;
        var jsonString = GetJsonString(selectedLanguage + ".json");
        Debug.Log("Loaded translations: " + jsonString);
        yield return null;
        translationsJSON = JSON.Parse(jsonString);
    }

    public string GetTranslation(string key, int index = 0)
    {
        // TODO: check if translations loaded before trying to get one


        // First find the correct translation for the given parameters
        var property = translationsJSON[key];

        if (property == null)
        {
            Debug.LogWarning("Translation missing for " + key);
            return null;
        }

        if (property.IsArray)
        {
            Debug.Log("Translation in array: " + property[index].Value);
            return property[index].Value;
        }
        else
        {
            Debug.Log("Translation: " + property.Value);
            return property.Value;
        }
    }

    public string GetJsonString(string filename)
    {
        string path = Application.streamingAssetsPath + "/Lang/" + filename;
        Debug.Log(path);

        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        return null;
    }
}
