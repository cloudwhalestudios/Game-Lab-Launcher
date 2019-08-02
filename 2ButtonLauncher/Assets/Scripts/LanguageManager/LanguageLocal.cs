using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LanguageLocal : MonoBehaviour
{
    public string key;
    public int index;
    public TextMeshProUGUI textSelf;

    private void Awake()
    {
        textSelf = GetComponent<TextMeshProUGUI>();
    }
}
