using System;
using Container;
using CoreGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI.PanelControllers
{
    public class ObserverPlayerStatElement : MonoBehaviour, IInventoryObserver
    {
        public Transform arrows;
        public TextMeshProUGUI playerTagText;
        public Image playerTagColor;
        
        private PlayerController _playerController;

        private void Start()
        {
            GUIEvents.current.OnPlayerReady += OnPlayerReady;
            GUIEvents.current.OnPlayerDone += OnPlayerDone;
        }

        private void OnPlayerDone(PlayerController player)
        {
            if (player == _playerController)
            {
                Destroy(gameObject);
            }
        }

        private void OnPlayerReady(bool state, PlayerTags player)
        {
            if (player == _playerController.playerTag)
            {
                if (state)
                {
                    playerTagText.text = _playerController.playerTag + " | Ready!";
                }
                else
                {
                    playerTagText.text = _playerController.playerTag + " | Not Ready";
                }
            }
        }

        public void SetPlayerController(PlayerController newPlayerController)
        {
            _playerController = newPlayerController;
            _playerController.AddInventoryObserver(this);
            playerTagText.text = _playerController.playerTag + " | Not Ready";
            playerTagColor.color = ColorPalette.current.GetPlayerColor(_playerController);
        }
        
        public void OnMoveInventoryChange(Direction[] directions)
        {
            GlobalMethods.UpdateArrows(arrows.transform,_playerController);
        }

        private void OnDestroy()
        {
            GUIEvents.current.OnPlayerReady -= OnPlayerReady;
        }
    }
}