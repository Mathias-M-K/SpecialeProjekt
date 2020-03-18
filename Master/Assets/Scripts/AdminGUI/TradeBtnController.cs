using UnityEngine;

namespace AdminGUI
{
    public class TradeBtnController : SendBtnController
    {
        [Range(0, 5)] public float PlayerPaletteAnimationSpeed;
        
        [Header("TradeBtn Settings")]
        public GameObject PlayerPalette;
        private bool playerPaletteActive;
        
        
        protected override void GUIButtonPressed(string key)
        {
            if (key.Equals("ManualControlBtn"))
            {
                enabledAndActive = true;
            }

            if (!enabledAndActive) return;
            if (key.Equals("TradeBtn"))
            {
                if (!active)
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
                if (active)
                {
                    if (!playerPaletteActive)
                    {
                        SetPlayerPaletteActive();
                    }
                }
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