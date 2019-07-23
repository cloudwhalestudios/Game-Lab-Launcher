using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public string selectedLang;
    public string backupLang = "Lan_EN";
    public 
    
    void Start()
    {
        
    }

    void Update()
    {
        if (selectedLang == null)
        {
            selectedLang = backupLang;
            Translate();
        }
    }

    public void Translate()
    {
        
    }
}
