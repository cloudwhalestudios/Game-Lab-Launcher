using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AccessibilityController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public static void AdjustSizeDelta(TextMeshProUGUI tmpText)
    {
        var sizeDelta = tmpText.rectTransform.sizeDelta;
        sizeDelta.x = tmpText.preferredWidth;
        tmpText.rectTransform.sizeDelta = sizeDelta;
    }

    public static void FindAndAdjustSizeDeltas(RectTransform container)
    {
        foreach (var tmpText in container.GetComponentsInChildren<TextMeshProUGUI>())
        {
            AdjustSizeDelta(tmpText);
        }
    }
}
