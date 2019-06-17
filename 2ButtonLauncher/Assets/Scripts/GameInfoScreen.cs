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
    public GameObject loadingIndicator;

    public bool IsVideoPlaying => videoGameTutorial.isPrepared && videoGameTutorial.isPlaying;

    public void ShowInfo(string developerTitle, string gameTitle, Sprite gameCover, string tutorialClipUrl, bool playOnLoad)
    {
        playVideosOnLoad = playOnLoad;
        SetupSimpleContent(developerTitle, gameTitle, gameCover);

        gameObject.SetActive(true);

        // Load and set content gallery
        SetupContentPreviewGallery(null, tutorialClipUrl);
    }
    public void ShowInfo(string developerTitle, string gameTitle, Sprite gameCover, VideoClip tutorialClip, bool playOnLoad)
    {
        playVideosOnLoad = playOnLoad;
        SetupSimpleContent(developerTitle, gameTitle, gameCover);

        gameObject.SetActive(true);

        // Loading and playing previews/tutorials
        SetupContentPreviewGallery(tutorialClip);
    }

    public void HideInfo()
    {
        playVideosOnLoad = false;
        videoGameTutorial.Stop();
        gameObject.SetActive(false);

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
            videoGameTutorial.url = tutorialClipURL;
        }
        else
        {
            videoGameTutorial.clip = tutorialClip;
        }
        StartCoroutine(LoadVideoRoutine());
    }

    IEnumerator LoadVideoRoutine()
    {
        videoGameTutorial.Prepare();
        loadingIndicator?.SetActive(true);
        while(!videoGameTutorial.isPrepared)
        {
            yield return null;
        }
        loadingIndicator?.SetActive(false);

        rawImageGameTutorial.texture = videoGameTutorial.texture;

        videoGameTutorial.isLooping = true;
        videoGameTutorial.Play();
        if (!playVideosOnLoad) videoGameTutorial.Pause();
    }

    public void PlayVideo()
    {
        if (!videoGameTutorial.isPlaying)
        {
            if (videoGameTutorial.isPrepared) videoGameTutorial.Play();
            else playVideosOnLoad = true;
        }
    }
    public void PauseVideo()
    {
        if (!videoGameTutorial.isPaused)
        {
            videoGameTutorial.Pause();
        }
    }
}
