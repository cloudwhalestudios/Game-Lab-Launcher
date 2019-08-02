using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public string selectedLang;
    public string backupLang = "Lang_EN";
    public LanguageLocal[] translatables;
    
    void Start()
    {
        // If no language is selected, use English.
        if (selectedLang == null || selectedLang == "")
        {
            selectedLang = backupLang;
            Translate();
        }
        else
        {
            Translate();
        }
        
    }
    
    //Find objects that contain LanguageLocal.cs (translatables var) and assign translation from the .json according to its textLine value.
    public void Translate()
    {
        string path;
        string updateText;
        
        translatables = FindObjectsOfType<LanguageLocal>();

        var jsonString = GetJsonString(selectedLang + ".json");
        Debug.Log(jsonString);

        var langJson = JSON.Parse(jsonString);

        //First find the correct translation for the object attached to LanguageLocal.cs
        for (var i = 0; i < translatables.Length; i++)
        {
            //Insert correct translation into var textSelf of LanguageLocal.cs
            var prop = langJson[translatables[i].key];

            if (prop == null)
            {
                Debug.LogWarning("Translation missing for " + translatables[i].key);
                continue;
            }
            
            if (prop.IsArray)
            {
                translatables[i].textSelf.text = prop[translatables[i].index].Value;
                Debug.Log(i + " Array: " + prop[translatables[i].index].Value);
            }
            else
            {
                translatables[i].textSelf.text = prop.Value;
                Debug.Log(i + " :" + prop.Value);
            }
        }
    }
    
    public string GetJsonString(string filename)
    {
        string path = Application.streamingAssetsPath + "/Lang/" + filename;
        Debug.Log(path);
        
        if(File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        return null;
    }
}
