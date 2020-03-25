using System;
using System.Security.Cryptography;
using CoreGame;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// ReSharper disable UseNullPropagation

namespace AdminGUI
{
    public class AdminGUIEvents : MonoBehaviour
    {
        public static AdminGUIEvents current;
        
        private Player CurrentChosenPlayer;
        
        private void Awake()
        {
            current = this;
        }

        public event Action<String> onButtonHit;
        public event Action<Player> onPlayerChange;
        public event Action<Button, TradeActions, Direction> onTradeAction;
        public event Action onManualOverride;
        public event Action onGameStart;

        //
        public void PlayerDropdown(TMP_Dropdown dropdown)
        {
            
            NotifyButtonHit("DropdownChanged");

            switch (dropdown.value)
            {
                case 0:
                    CurrentChosenPlayer = Player.Red;
                    break;
                case 1:
                    CurrentChosenPlayer = Player.Blue;
                    break;
                case 2:
                    CurrentChosenPlayer = Player.Green;
                    break;
                case 3:
                    CurrentChosenPlayer = Player.Yellow;
                    break;
            }

            dropdown.image.color = ColorPalette.current.GetPlayerColor(CurrentChosenPlayer);
            OnPlayerChange();
        }
        /*
         * GAME BUTTONS
         */
        //Start game
        public void StartGame()
        {
            GameHandler.current.StartGame();
            if (onGameStart != null) onGameStart();
        }

        public void BtnHit(Button b)
        {
            NotifyButtonHit(b.name);
        }

        /*
         * ADMIN GUI BUTTONS
         */
        //Manual Control Button
        public void ManualControl(Button b)
        {
            b.interactable = false;
            OnPlayerChange();
            
            ManualEnabledNotify();
        }
        
        //Ready Button
        public void ReadyBtnHit()
        {
            NotifyButtonHit("ReadyBtn");
        }

        //Prime Buttons
        public void SendBtnHit()
        {
            NotifyButtonHit("SendBtn");
        }

        public void TradeBtnHit()
        {
            NotifyButtonHit("TradeBtn");
        }
        
        //Arrow
        public void Arrow(Button b)
        {
            NotifyButtonHit(b.name);
        }
      

        //Colors
        public void ColorFirst()
        {
            NotifyButtonHit("Color0");
        }

        public void ColorSecond()
        {
            NotifyButtonHit("Color1");
        }

        public void ColorThird()
        {
            NotifyButtonHit("Color2");
        }

        //Incoming Trade Buttons
        public void IncomingTradeBtn(Button b)
        {
            NotifyButtonHit(b.name);

        }

        
          //Outgoing Trade Buttonn
          public void OutGoingTradeBtn(Button b)
        {
            NotifyButtonHit(b.name);
        }

        
        //Accept, Reject & Cancel
        public void AcceptBtn()
        {
            NotifyButtonHit("AcceptBtn");
        }

        public void RejectBtn()
        {
            print(EventSystem.current.currentSelectedGameObject.name);
            NotifyButtonHit("RejectBtn");
        }

        public void CancelBtn()
        {
            NotifyButtonHit("CancelBtn");
        }

        //Notify Methods
        public void TradeActionNotify(Button b, TradeActions action, Direction counterOffer)
        {
            if (onTradeAction != null) onTradeAction(b, action, counterOffer);
        }

        private void ManualEnabledNotify()
        {
            if (onManualOverride != null) onManualOverride();
        }

        private void NotifyButtonHit(string key)
        {
            if (onButtonHit != null) onButtonHit(key);
        }

        private void OnPlayerChange()
        {
            if (onPlayerChange != null) onPlayerChange(CurrentChosenPlayer);
        }
    }
}