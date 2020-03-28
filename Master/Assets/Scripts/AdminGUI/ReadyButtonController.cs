using System;
using CoreGame;
using CoreGame.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdminGUI
{
    public class ReadyButtonController : MonoBehaviour,IReadyObserver
    {
        public Button readyBtn;
        private TextMeshProUGUI text;

        private PlayerController _playerController;

        private ColorBlock redColors;
        private ColorBlock greenColors;
        
        private void Start()
        {
            redColors = ColorPalette.current.yellowButton;
            greenColors = ColorPalette.current.greenButton;
            
            text = readyBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

            GUIEvents.current.onButtonHit += OnBtnHit;
            GUIEvents.current.onPlayerChange += OnPlayerChange;
        }

        private void OnPlayerChange(Player player)
        {
            if (_playerController != null)
            {
                _playerController.RemoveReadyObserver(this);
            }

            _playerController = GameHandler.current.GetPlayerController(player);
            _playerController.AddReadyObserver(this);

            if (_playerController.Ready)
            {
                SetReady();
            }
            else
            {
                SetUnready();
            }
        }
        
        private void OnBtnHit(Button button)
        {
            string key = button.name;
            if (key.Equals("ReadyBtn"))
            {
                _playerController.Ready = !_playerController.Ready;
            }
            EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
        }
        
        private void SetReady()
        {
            ColorBlock colorBlock = readyBtn.colors;
            
            text.text = "READY!";
            colorBlock = greenColors;

            readyBtn.colors = colorBlock;
        }

        private void SetUnready()
        {
            ColorBlock colorBlock = readyBtn.colors;

            colorBlock = redColors;
            text.text = "Ready?";
            readyBtn.colors = colorBlock;
        }

        public void OnReadyStateChanged(bool state)
        {
            switch (state)
            {
                case true:
                    SetReady();
                    break;
                case false:
                    SetUnready();
                    break;
            }
        }
    }
}