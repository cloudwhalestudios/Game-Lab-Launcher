using System.Collections;
using UnityEngine;
using WebGLIntegration;

public class ExitController : MonoBehaviour
{
    [SerializeField] private float timeUntilRedirect = 5f;
    [SerializeField] private RectTransform timeIndicator;

    // Start is called before the first frame update
    void Start()
    {
        timeIndicator.localScale = Vector3.one;

        WebGLSite.ActivateExitCondition();

        StartCoroutine(HandleBrowserExitRoutine());
    }

    IEnumerator HandleBrowserExitRoutine()
    {
        var elapsedTime = 0f;

        while (true)
        {
            if (timeIndicator != null)
            {
                if (elapsedTime >= timeUntilRedirect) break;

                var percentage = Mathf.Clamp01(elapsedTime / timeUntilRedirect);
                timeIndicator.localScale = new Vector3(1 - percentage, 1, 1);
                yield return new WaitForEndOfFrame();
                elapsedTime += Time.unscaledDeltaTime;
            }
            else
            {
                yield return new WaitForSecondsRealtime(timeUntilRedirect);
                break;
            }
        }

        PlatformManager.Instance.Exit();
    }
}
