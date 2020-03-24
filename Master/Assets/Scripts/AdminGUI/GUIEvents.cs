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
    public class GUIEvents : MonoBehaviour
    {
        public static GUIEvents current;

        public TMP_Dropdown playerDropdown;
        public Button ManualControlBtn;
        public Player currentChosenPlayer;

        private void Awake()
        {
            current = this;
        }

        public event Action<String> onButtonHit;
        public event Action<Player> onPlayerChange;
        public event Action<Button, TradeActions, Direction> onTradeAction;
        public event Action onManualOverride;


        /*
         * Manual Control Button
         */
        public void ManualControl()
        {
            PlayerDropdownChanged();


            //LeanTween.value(ManualControlBtn.GetComponent<RectTransform>().sizeDelta.x,0,0.5f).setOnUpdate(SetWidthOnBtn).setOnComplete(() => Destroy(ManualControlBtn.gameObject));

            ManualEnabledNotify();
        }

        private void SetWidthOnBtn(float width)
        {
            ManualControlBtn.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,width);
        }
        
        /*
         * Ready Button
         */
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

        /*
         * Incomming Trade Buttons
         */
        public void IncomingTradeBtn(Button b)
        {
            NotifyButtonHit(b.name);

        }

        /*
         * Outgoing Trade Buttons
         */
        public void OutGoingTradeBtn(Button b)
        {
            NotifyButtonHit(b.name);
        }

        /*
         * Accept, Reject & Cancel
         */

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

        /*
         * Notify Methods
         */
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

        public void PlayerDropdownChanged()
        {
            NotifyButtonHit("DropdownChanged");

            switch (playerDropdown.value)
            {
                case 0:
                    currentChosenPlayer = Player.Red;
                    break;
                case 1:
                    currentChosenPlayer = Player.Blue;
                    break;
                case 2:
                    currentChosenPlayer = Player.Green;
                    break;
                case 3:
                    currentChosenPlayer = Player.Yellow;
                    break;
            }

            playerDropdown.image.color = ColorPalette.current.GetPlayerColor(currentChosenPlayer);

            if (onPlayerChange != null) onPlayerChange(currentChosenPlayer);
        }
    }

    public enum Keys
    {
        Mathias,
        ikkeMathias
        
    }
}