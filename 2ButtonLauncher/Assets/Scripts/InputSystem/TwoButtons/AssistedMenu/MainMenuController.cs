using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AccessibilityInputSystem
{
    namespace TwoButtons
    {
        public class MainMenuController : BaseMenuController
        {
            public bool enableAutomaticIndication = false;
            [SerializeField, ReadOnly] private bool isReady = false;

            public void Start()
            {
                ResetController();
                if (BasePlayerManager.Instance.PlayerCount > 0)
                {
                    isReady = true;
                    MenuManager.Instance.StartIndicating();
                }
            }

            public void ResetController()
            {
                MenuManager.Instance.SetActiveMenu(this);
                if (itemSelectIndicator != null)
                {
                    itemSelectIndicator.gameObject.SetActive(false);
                    
                }
                if (itemSelectTimer != null) itemSelectTimer.localScale = new Vector3(0, 1, 1);

                menuContainer.SetActive(false);
            }

            void OnEnable()
            {
                if (enableAutomaticIndication)
                {
                    BasePlayerManager.NewPlayerAdded += PlayerManager_NewPlayerAdded;
                    BasePlayerManager.PlayerRemoved += PlayerManager_PlayerWasRemoved;
                }
            }

            void OnDisable()
            {
                if (enableAutomaticIndication)
                {
                    BasePlayerManager.NewPlayerAdded -= PlayerManager_NewPlayerAdded;
                    BasePlayerManager.PlayerRemoved -= PlayerManager_PlayerWasRemoved;
                }
            }

            private void PlayerManager_NewPlayerAdded(BasePlayer player)
            {
                if (isReady) return;
                isReady = true;
                MenuManager.Instance.StartIndicating();
            }

            private void PlayerManager_PlayerWasRemoved(int total)
            {
                if (total != 0) return;
                isReady = false;
                MenuManager.Instance.StartIndicating(false);
            }
        }
    }
}