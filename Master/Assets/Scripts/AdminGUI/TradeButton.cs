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
        private PlayerTags[] colorOrder = new PlayerTags[3];

        protected override void Start()
        {
            base.Start();
            GUIEvents.current.onPlayerChange += UpdatePlayerPalette;
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
                    _playerController.CreateTrade(_activeDirection,colorOrder[indexFetchValue]);
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
            int i = 0;
            foreach (PlayerController controller in GameHandler.Current.GetPlayers())
            {
                if (controller == _playerController) continue;

                Image img = playerPalette.transform.GetChild(i).GetComponent<Image>();

                img.color = ColorPalette.current.GetPlayerColor(controller.playerTags);

                colorOrder[i] = controller.playerTags;
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
            LeanTween.moveLocalY(playerPalette, 80, playerPaletteAnimationSpeed).setOnComplete(SetArrowsInactive);
            _playerPaletteActive = false;
        }
        
        
    }
}