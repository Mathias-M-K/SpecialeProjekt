using System;
using System.Collections.Generic;
using System.Globalization;
using CoreGame;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    public class TradeButton : _PrimeButton
    {
        [Range(0, 5)] public float playerPaletteAnimationSpeed;
        
        [Header("TradeBtn Settings")]
        public GameObject playerPalette;
        
        private bool _playerPaletteActive;
        private Direction _activeDirection;
        private readonly PlayerTags[] _colorOrder = new PlayerTags[16];

        private PlayerTags _currentPlayer;

        protected override void Start()
        {
            base.Start();
            GUIEvents.current.onPlayerChange += UpdatePlayerPalette;
            GUIEvents.current.OnGameStart += OnGameStart;
        }

        private void OnGameStart()
        {
            UpdatePlayerPalette(_currentPlayer);
        }

        protected override void GUIButtonPressed(Button button)
        {
            string key = button.name;
            if (!enabledAndActive) return;
            
            if (key.Equals("TradeBtn"))
            {
                if (!arrowsActive)
                {
                    SetArrowsActive();
                }
                else
                {
                    if (_playerPaletteActive)
                    {
                        SetPlayerPaletteInactive();
                    }
                    else
                    {
                        SetArrowsInactive();
                    }
                }
            } else if (key.Substring(0, 5).Equals("Arrow"))
            {
                if (arrowsActive)
                {
                    int.TryParse(key.Substring(5, key.Length - 5), out int indexFetchValue);
                    _activeDirection = _playerController.GetMoves()[indexFetchValue];
                    
                    if (!_playerPaletteActive)
                    {
                        SetPlayerPaletteActive();
                    }
                }
            }else if(key.Substring(0, 5).Equals("Color"))
            {
                if (_playerPaletteActive)
                {
                    int.TryParse(key.Substring(5, key.Length - 5), out int indexFetchValue);
                    _playerController.CreateTrade(_activeDirection,_colorOrder[indexFetchValue]);
                    _playerController.NotifyInventoryObservers();
                    SetPlayerPaletteInactive();
                }
                else
                {
                    SetArrowsInactive();
                }
            }
            else
            {
                SetPlayerPaletteInactive();
            }
        }

        private void UpdatePlayerPalette(PlayerTags playerTags)
        {
            _currentPlayer = playerTags;
            
            for (int j = 0; j < 16; j++)
            {
                GetPlayerColor(j).GetComponent<Image>().color = new Color32(0,0,0,0);
                
            }
            
            int i = 0;
            foreach (PlayerController controller in GameHandler.Current.GetPlayers())
            {
                if (controller == _playerController) continue;

                Image img = GetPlayerColor(i).GetComponent<Image>();

                img.color = ColorPalette.current.GetPlayerColor(controller.playerTag);

                _colorOrder[i] = controller.playerTag;
                i++;
            }
        }

        private void SetPlayerPaletteActive()
        {
            LeanTween.moveLocalY(playerPalette, 0, playerPaletteAnimationSpeed);
            _playerPaletteActive = true;
        }

        private void SetPlayerPaletteInactive()
        {
            LeanTween.moveLocalY(playerPalette, 397, playerPaletteAnimationSpeed).setOnComplete(SetArrowsInactive);
            _playerPaletteActive = false;
        }

        private Transform GetPlayerColor(float i)
        {
            const int numberOfRows = 4;
            const int elementsInRow = 4;

            int row = Mathf.FloorToInt(i / numberOfRows);
            int element = Mathf.FloorToInt(i % elementsInRow);
            
            Transform tRow = playerPalette.transform.GetChild(row);
            Transform tElement = tRow.GetChild(element);
            
            print($"row: {row}, Element: {element}");

            return tElement;
        }
        
        
    }
}