using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class TextResizer : MonoBehaviour
{
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
