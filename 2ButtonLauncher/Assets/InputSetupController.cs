using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputSetupController : MonoBehaviour
{

    public TextMeshProUGUI TextScreenPrompt;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InputSetupRoutine());
    }

    private IEnumerator InputSetupRoutine()
    {
        // Reset
        ResetConfiguration();
        // Welcome

        // First button

        // Confirm

        // Second Button

        // Confirm

        yield break;
    }

    private void ResetConfiguration()
    {
        throw new NotImplementedException();
    }
}
