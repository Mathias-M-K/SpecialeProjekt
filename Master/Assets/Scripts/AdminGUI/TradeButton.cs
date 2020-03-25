using CoreGame;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    public class TradeButton : _PrimeButton
    {
        [Range(0, 5)] public float PlayerPaletteAnimationSpeed;
        
        [Header("TradeBtn Settings")]
        public GameObject PlayerPalette;
        
        private bool playerPaletteActive;
        private Direction activeDirection;
        private Player[] colorOrder = new Player[3];

        protected override void Start()
        {
            base.Start();
            AdminGUIEvents.current.onPlayerChange += UpdatePlayerPalette;
        }

        protected override void GUIButtonPressed(string key)
        {
            if (!enabledAndActive) return;
            
            if (key.Equals("TradeBtn"))
            {
                if (!arrowsActive)
                {
                    SetArrowsActive();
                }
                else
                {
                    if (playerPaletteActive)
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
                    activeDirection = _playerController.GetMoves()[indexFetchValue];
                    
                    if (!playerPaletteActive)
                    {
                        SetPlayerPaletteActive();
                    }
                }
            }else if(key.Substring(0, 5).Equals("Color"))
            {
                if (playerPaletteActive)
                {
                    int.TryParse(key.Substring(5, key.Length - 5), out int indexFetchValue);
                    _playerController.CreateTrade(activeDirection,colorOrder[indexFetchValue]);
                    _playerController.NotifyMoveObservers();
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

        private void UpdatePlayerPalette(Player player)
        {
            int i = 0;
            foreach (PlayerController controller in GameHandler.current.GetPlayers())
            {
                if (controller == _playerController) continue;

                Image img = PlayerPalette.transform.GetChild(i).GetComponent<Image>();

                img.color = ColorPalette.current.GetPlayerColor(controller.player);

                colorOrder[i] = controller.player;
                i++;
            }
        }

        private void SetPlayerPaletteActive()
        {
            LeanTween.moveLocalY(PlayerPalette, 0, PlayerPaletteAnimationSpeed);
            playerPaletteActive = true;
        }

        private void SetPlayerPaletteInactive()
        {
            LeanTween.moveLocalY(PlayerPalette, 80, PlayerPaletteAnimationSpeed).setOnComplete(SetArrowsInactive);
            playerPaletteActive = false;
        }
        
        
    }
}