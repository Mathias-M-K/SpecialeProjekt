﻿using System;
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

        /*
        private Color32 colorReady = new Color32(120, 224, 143,255);
        private Color32 colorReadyHighlight = new Color32(184, 233, 148,255);
        
        private Color32 colorUnready = new Color32(246, 185, 59,255);
        private Color32 colorUnreadyHighlight = new Color32(250, 211, 144,255);
        */
        
        
        [SerializeField]private Color32 colorReady = new Color32(106, 176, 76,255);
        private Color32 colorReadyHighlight = new Color32(186, 220, 88,255);
        
        private Color32 colorUnready = new Color32(249, 202, 36,255);
        private Color32 colorUnreadyHighlight = new Color32(249, 202, 36,255);
        
        private void Start()
        {
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
        
        private void OnBtnHit(string key)
        {
            if (key.Equals("ReadyBtn"))
            {
                _playerController.Ready = !_playerController.Ready;
            }
            EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
        }
        [ContextMenu("HEYO")]
        private void SetReady()
        {
            ColorBlock colorBlock = readyBtn.colors;
            
            colorBlock.normalColor = colorReady;
            colorBlock.highlightedColor = colorReadyHighlight;

            text.text = "READY!";

            readyBtn.colors = colorBlock;
        }

        private void SetUnready()
        {
            ColorBlock colorBlock = readyBtn.colors;
            
            colorBlock.normalColor = colorUnready;
            colorBlock.highlightedColor = colorUnreadyHighlight;

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