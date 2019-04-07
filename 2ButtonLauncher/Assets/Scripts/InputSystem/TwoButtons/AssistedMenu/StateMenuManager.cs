﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AccessibilityInputSystem
{
    namespace TwoButtons
    {
        public class StateMenuManager : MonoBehaviour
        {
            public static StateMenuManager Instance { get; private set; }

            public float autoInterval;
            public bool hideHighlightOnSelect = true;
            public int direction = 1;

            [SerializeField, ReadOnly] private BaseStateMenuController controller;
            [SerializeField, ReadOnly] private int selectedStateIndex;

            Coroutine stateSelector;

            StateMenu lastHighlightedState;

            BaseStateMenuController.Mode currentMode;

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

            public void SetStateMenuController(BaseStateMenuController controller)
            {
                this.controller = controller;
                currentMode = controller.indicatorMode;
            }

            void Cleanup() => StopAllCoroutines();

            void OnDestroy()
            {
                if (Instance == this) { Instance = null; }
                Cleanup();
            }

            public void Select()
            {
                Debug.Log($"Selecting State {controller.stateMenus[selectedStateIndex].name} ({selectedStateIndex})");

                switch (currentMode)
                {
                    case BaseStateMenuController.Mode.Single:
                        // Select button from active menu
                        MenuManager.Instance.SelectItem();
                        break;

                    case BaseStateMenuController.Mode.State:
                        // Stop state indication
                        StartIndicating(false);
                        if (hideHighlightOnSelect)
                        {
                            lastHighlightedState.highlight.SetActive(false);
                        }

                        // Start menu indication
                        currentMode = BaseStateMenuController.Mode.Single;

                        var selectedState = controller.stateMenus[selectedStateIndex];
                        selectedState.selectEvent?.Invoke();

                        MenuManager.Instance.SetMenuController(selectedState.menuController);
                        StartIndicating();
                        break;
                }
            }

            public void Return()
            {
                switch (currentMode)
                {
                    case BaseStateMenuController.Mode.Single:
                        // Stop menu indication
                        StartIndicating(false);

                        var selectedState = controller.stateMenus[selectedStateIndex];
                        selectedState.returnEvent?.Invoke();

                        // Start state indication
                        currentMode = BaseStateMenuController.Mode.State;

                        if (hideHighlightOnSelect)
                        {
                            lastHighlightedState.highlight.SetActive(true);
                        }

                        StartIndicating();
                        break;
                    case BaseStateMenuController.Mode.State:
                        // TODO ??

                        break;
                }
            }

            public void StartIndicating(bool start = true)
            {
                switch (currentMode)
                {
                    case BaseStateMenuController.Mode.Single:
                        MenuManager.Instance.StartIndicating(start);
                        break;
                    case BaseStateMenuController.Mode.State:
                        // Start state indication
                        if (start)
                        {
                            selectedStateIndex = Mathf.Clamp(controller.startStateIndex, 0, controller.stateMenus.Count - 1);
                            stateSelector = StartCoroutine(StateSelection());
                        }
                        else
                        {
                            Cleanup();
                            if (stateSelector != null) stateSelector = null;
                        }
                        break;
                }
            }

            IEnumerator StateSelection()
            {
                StateMenu selectedState;
                selectedStateIndex = Mathf.Clamp(selectedStateIndex, 0, controller.stateMenus.Count - 1);
                yield return null;
                HighlightState(controller.stateMenus[selectedStateIndex]);

                while (true)
                {
                    selectedState = controller.stateMenus[selectedStateIndex];

                    HighlightState(selectedState);


                    if (selectedState.stateTimer != null)
                    {
                        yield return StartCoroutine(UpdateTimerProgress(selectedState.stateTimer, autoInterval));
                    }
                    else
                    {
                        yield return new WaitForSecondsRealtime(autoInterval);
                    }

                    selectedStateIndex = (selectedStateIndex + controller.stateMenus.Count + direction) % controller.stateMenus.Count;
                }
            }

            IEnumerator UpdateTimerProgress(RectTransform timer, float waitTime)
            {
                //Debug.Log("Timer ...");
                var elapsedTime = 0f;
                timer.localScale = new Vector3(0, 1, 1);
                while (elapsedTime < waitTime)
                {
                    yield return null;
                    elapsedTime += Time.unscaledDeltaTime;
                    var percentage = Mathf.Clamp01(elapsedTime / waitTime);
                    //Debug.Log($"Percent: {percentage * 100f}%; Elapsed: {elapsedTime}s; Wait: {waitTime}s");
                    timer.localScale = new Vector3(percentage, 1, 1);
                }
            }

            void HighlightState(StateMenu state)
            {
                if (lastHighlightedState != null)
                {
                    lastHighlightedState.highlight.SetActive(false);
                }
                state.highlight.SetActive(true);
                lastHighlightedState = state;

            }
            
        }
    }
}