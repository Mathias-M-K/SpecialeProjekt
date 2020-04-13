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
        
        public void SetPlayerController(PlayerController newPlayerController)
        {
            _playerController = newPlayerController;
            _playerController.AddInventoryObserver(this);
            playerTagText.text = newPlayerController.playerTag.ToString();
            playerTagColor.color = ColorPalette.current.GetPlayerColor(_playerController);
        }
        
        public void OnMoveInventoryChange(Direction[] directions)
        {
            GlobalMethods.UpdateArrows(arrows.transform,_playerController);
        }
    }
}