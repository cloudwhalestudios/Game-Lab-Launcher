using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AccessibilityInputSystem
{
    namespace TwoButtons
    {
        public class MenuManager : MonoBehaviour
        {
            public static MenuManager Instance { get; private set; }

            public float autoInterval;
            public bool updateStartIndex = false;
            public int direction = 1;

            [SerializeField, ReadOnly] private BaseMenuController controller;
            [SerializeField, ReadOnly] private int currentIndex;
            [SerializeField, ReadOnly] private List<Button> buttons;

            Coroutine menuSelector;

            Sprite lastDefaultStateSprite;
            Color lastDefaultColor;

            void Awake()
            {
                if (Instance == null)
                {
                    Instance = this;
                    DontDestroyOnLoad(this);
                }
                else
                {
                    DestroyImmediate(gameObject);
                }
            }

            private void OnEnable() => SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
            private void OnDisable() => SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;

            private void SceneManager_activeSceneChanged(Scene from, Scene to) => Cleanup();

            public void SetMenuController(BaseMenuController menuController) => controller = menuController;
            void Cleanup() => StopAllCoroutines();

            void OnDestroy()
            {
                if (Instance == this) { Instance = null; }
                Cleanup();
            }

            public void ShowMenu(bool startMoving = true)
            {
                if (controller?.menuContainer != null)
                {
                    controller.menuContainer.SetActive(true);
                    StartIndicating(startMoving);
                }
            }
            public void HideMenu()
            {
                if (controller?.menuContainer != null)
                {
                    controller.menuContainer.SetActive(false);
                    StartIndicating(false);
                }
            }

            public void StartIndicating(bool indicate = true)
            {
                if (indicate)
                {
                    if (controller.itemSelectIndicator != null) controller.itemSelectIndicator.gameObject.SetActive(true);
                    if (controller.transitionType == BaseMenuController.Transition.Animate) UpdateAnimationSpots();
                    menuSelector = StartCoroutine(MenuSelection());
                }
                else
                {
                    if (controller.itemSelectIndicator != null) controller.itemSelectIndicator?.gameObject.SetActive(false);
                    if (updateStartIndex) controller.startingIndex = currentIndex;

                    if (menuSelector != null)
                    {
                        Cleanup();
                        menuSelector = null;
                    }
                    HighlightButton(buttons[currentIndex], true);
                }
            }

            private void UpdateAnimationSpots()
            {
                var firstIndex = (currentIndex + buttons.Count - direction) % buttons.Count;
                var lastIndex = (currentIndex + buttons.Count + direction) % buttons.Count;

                controller.firstSpot.buttonObject = buttons[firstIndex].gameObject;
                controller.currentSpot.buttonObject = buttons[currentIndex].gameObject;
                controller.lastSpot.buttonObject = buttons[lastIndex].gameObject;
            }

            public void SelectItem()
            {
                buttons[currentIndex].onClick.Invoke();
            }

            IEnumerator MenuSelection()
            {
                Button selectedButton;
                buttons = new List<Button>(controller.buttonParent.GetComponentsInChildren<Button>());
                currentIndex = Mathf.Clamp(controller.startingIndex, 0, buttons.Count - 1);

                yield return null;
                HighlightButton(buttons[currentIndex]);

                while (true)
                {
                    selectedButton = buttons[currentIndex];

                    // Indicate and Highlight
                    switch (controller.transitionType)
                    {
                        case BaseMenuController.Transition.Move:
                            IndicateButton(selectedButton);
                            break;
                        case BaseMenuController.Transition.Animate:
                            AnimateButton(selectedButton);
                            break;
                        default:
                            break;
                    }

                    HighlightButton(selectedButton);
                    
                    if (controller.itemSelectTimer != null)
                    {
                        yield return StartCoroutine(UpdateTimerProgress(autoInterval));
                    }
                    else
                    {
                        yield return new WaitForSecondsRealtime(autoInterval);
                    }

                    HighlightButton(selectedButton, true);
                    currentIndex = (currentIndex + buttons.Count + direction) % buttons.Count;
                }
            }

            private void AnimateButton(Button selectedButton)
            {
                try
                {
                    if (selectedButton.gameObject.Equals(controller.currentSpot.buttonObject)) return;
                    StartCoroutine(AnimateButtonCoroutine());
                }
                catch (Exception)
                {

                    return;
                }
            }

            IEnumerator AnimateButtonCoroutine()
            {
                // Fade last button
                AnimateMoveItem(controller.lastSpot.buttonObject, 
                    controller.lastSpot.location, 
                    controller.disappearClip, 
                    controller.transitionTime);
                // Move and downscale last current button
                AnimateMoveItem(controller.currentSpot.buttonObject,
                    controller.lastSpot.location,
                    controller.shrinkClip,
                    controller.transitionTime);

                // Move and upscale new current button
                AnimateMoveItem(controller.firstSpot.buttonObject,
                    controller.currentSpot.location,
                    controller.growClip,
                    controller.transitionTime);

                UpdateAnimationSpots();

                // Fade in new button
                AnimateMoveItem(controller.firstSpot.buttonObject,
                    controller.firstSpot.location,
                    controller.appearClip,
                    controller.transitionTime);

                yield return null;

            }

            IEnumerator AnimateMoveItem(GameObject item, Vector3 location, AnimationClip clip, float transitionTime)
            {
                var animation = item.GetComponent<Animation>();
                if (animation == null)
                {
                    animation = item.AddComponent<Animation>();
                }
                if (animation.GetClip(clip.name) == null)
                {
                    animation.AddClip(clip, clip.name);
                }

                animation.Play(clip.name);
                animation[clip.name].speed = clip.length / transitionTime;

                var startPos = item.transform.localPosition;
                if (startPos == location) yield break;

                var elapsedTime = 0f;
                while (elapsedTime <= transitionTime)
                {
                    yield return null;
                    elapsedTime += Time.unscaledDeltaTime;
                    var percentage = Mathf.Clamp01(elapsedTime / transitionTime);
                    item.transform.localPosition = Vector3.Lerp(startPos, location, percentage);
                }
            }

            IEnumerator UpdateTimerProgress(float waitTime)
            {
                //Debug.Log("Timer ...");
                var elapsedTime = 0f;
                controller.itemSelectTimer.localScale = new Vector3(0, 1, 1);
                while (elapsedTime < waitTime)
                {
                    yield return null;
                    elapsedTime += Time.unscaledDeltaTime;
                    var percentage = Mathf.Clamp01(elapsedTime / waitTime);
                    //Debug.Log($"Percent: {percentage * 100f}%; Elapsed: {elapsedTime}s; Wait: {waitTime}s");
                    controller.itemSelectTimer.localScale = new Vector3(percentage, 1, 1);
                }
            }

            void IndicateButton(Button btn)
            {
                try
                {
                    var btnRect = btn.GetComponent<RectTransform>();
                    var posX = new Vector2(btnRect.localPosition.x, 0);
                    var posY = new Vector2(0, btnRect.localPosition.y);

                    if (controller.itemSelectIndicator != null)
                        controller.itemSelectIndicator.anchoredPosition = posX + posY + controller.itemIndicatorOffset;
                   
                }
                catch (Exception)
                {
                    return;
                }
            }

            void HighlightButton(Button btn, bool revert = false)
            {
                try
                {
                    if (btn.transition == Selectable.Transition.SpriteSwap)
                    {
                        if (revert && btn?.image?.sprite != null)
                        {
                            if (lastDefaultStateSprite != null) btn.image.sprite = lastDefaultStateSprite;
                            if (lastDefaultColor != null) btn.image.color = lastDefaultColor;
                            return;
                        }

                        lastDefaultStateSprite = btn.image.sprite;
                        lastDefaultColor = btn.image.color;
                        btn.image.sprite = btn.spriteState.highlightedSprite;
                        btn.image.color = Color.white;
                    }
                    else if (btn.transition == Selectable.Transition.Animation)
                    {
                        if (btn?.animator != null)
                        {
                            if (revert && btn?.animationTriggers?.normalTrigger != null)
                            {
                                btn.animator.SetBool(btn.animationTriggers.normalTrigger, true);
                                return;
                            }
                            btn.animator.SetBool(btn.animationTriggers.highlightedTrigger, true);
                        }
                    }
                }
                catch (Exception)
                {

                    return;
                }
            }
        }
    }
}