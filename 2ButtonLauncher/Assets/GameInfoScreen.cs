using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;
using System;

public class GameInfoScreen : MonoBehaviour
{
    [Header("Text Areas")]
    public TextMeshProUGUI textDeveloperTitle;
    public TextMeshProUGUI textGameTitle;

    [Header("Content Areas")]
    public Image imageGameCover;

    [Space]
    public VideoPlayer videoGameTutorial;
    public RawImage rawImageGameTutorial;
    public bool playVideosOnLoad = false;

    public void SetDisplayInfo(string developerTitle, string gameTitle, Sprite gameCover, string tutorialClipUrl, bool playOnLoad)
    {
        playVideosOnLoad = playOnLoad;
        SetupSimpleContent(developerTitle, gameTitle, gameCover);

        // Load and set content gallery
        SetupContentPreviewGallery(null, tutorialClipUrl);
    }
    public void SetDisplayInfo(string developerTitle, string gameTitle, Sprite gameCover, VideoClip tutorialClip, bool playOnLoad)
    {
        playVideosOnLoad = playOnLoad;
        SetupSimpleContent(developerTitle, gameTitle, gameCover);

        // Loading and playing previews/tutorials
        SetupContentPreviewGallery(tutorialClip);
    }

    void SetupSimpleContent(string developerTitle, string gameTitle, Sprite gameCover)
    {
        // Setting text
        textDeveloperTitle.text = developerTitle;
        textGameTitle.text = gameTitle;

        // Setting images
        imageGameCover.sprite = gameCover;

        // Load and set categories
        SetupCategories();
    }

    private void SetupCategories()
    {
        // TODO Implement categories
    }

    private void SetupContentPreviewGallery(VideoClip tutorialClip, string tutorialClipURL = null)
    {
        // TODO create steam like content gallery

        // Load and play previews/tutorials
        if (tutorialClip == null)
        {
            if (tutorialClipURL == null)
            {
                Debug.LogWarning("Expected tutorial video or it's url, but got nothing instead!");
                return;
            }
            SetupVideoFromUrl(tutorialClipURL, rawImageGameTutorial);
        }
        else
        {
            StartCoroutine(LoadVideoRoutine(tutorialClip));
        }
    }

    private void SetupVideoFromUrl(string tutorialClipURL, RawImage rawImageGameTutorial)
    {
        throw new NotImplementedException();
    }

    IEnumerator LoadVideoRoutine(VideoClip clip)
    {
        videoGameTutorial.clip = clip;
        videoGameTutorial.Prepare();

        while(!videoGameTutorial.isPrepared)
        {
            yield return null;
        }

        rawImageGameTutorial.texture = videoGameTutorial.texture;

        if (playVideosOnLoad) videoGameTutorial.Play();
    }
}
