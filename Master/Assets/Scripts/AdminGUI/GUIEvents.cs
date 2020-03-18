using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using CoreGame;
using TMPro;
using UnityEngine;
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
            ManualBtnState1();
        }

        public event Action<String> onButtonHit;
        public event Action<Player> onPlayerChange;

        public void PlayerDropdownChanged()
        {
            NotifyAll("DropdownChanged");

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

            playerDropdown.image.color = GameHandler.current.GetPlayerMaterial(currentChosenPlayer).color;

            if (onPlayerChange != null) onPlayerChange(currentChosenPlayer);
        }

        private void ManualBtnState1()
        {
            Image img = ManualControlBtn.GetComponent<Image>();
            LeanTween.color(img.rectTransform, new Color32(96, 163, 188, 255), 1f).setOnComplete(ManualBtnState2)
                .setEase(LeanTweenType.easeOutSine);
        }

        private void ManualBtnState2()
        {
            Image img = ManualControlBtn.GetComponent<Image>();
            LeanTween.color(img.rectTransform, new Color32(10, 61, 98, 255), 1f).setOnComplete(ManualBtnState1)
                .setEase(LeanTweenType.easeOutSine);
        }


        /*
         * Manual Control Button
         */
        public void ManualControl()
        {
            PlayerDropdownChanged();

            LeanTween.alpha(ManualControlBtn.GetComponent<Image>().rectTransform, 0, 0.2f).destroyOnComplete = true;
            NotifyAll("ManualControlBtn");
        }

        /*
         * Prime Buttons
         */
        public void SendBtnHit()
        {
            NotifyAll("SendBtn");
        }

        public void TradeBtnHit()
        {
            NotifyAll("TradeBtn");
        }

        /*
         * Arrows
         */
        public void ArrowFirst()
        {
            NotifyAll("ArrowFirst");
        }

        public void ArrowSecond()
        {
            NotifyAll("ArrowSecond");
        }

        public void ArrowThird()
        {
            NotifyAll("ArrowThird");
        }

        public void ArrowForth()
        {
            NotifyAll("ArrowForth");
        }

        /*
         * Incomming Trade Buttons
         */

        public void IncomingTradeBtn1()
        {
            NotifyAll("IncomingTradeBtn1");
        }

        public void IncomingTradeBtn2()
        {
            NotifyAll("IncomingTradeBtn2");
        }

        public void IncomingTradeBtn3()
        {
            NotifyAll("IncomingTradeBtn3");
        }

        public void IncomingTradeBtn4()
        {
            NotifyAll("IncomingTradeBtn4");
        }

        /*
         * Outgoing Trade Buttons
         */

        public void OutgoingTradeBtn1()
        {
            NotifyAll("OutgoingTradeBtn1");
        }

        public void OutgoingTradeBtn2()
        {
            NotifyAll("OutgoingTradeBtn2");
        }

        public void OutgoingTradeBtn3()
        {
            NotifyAll("OutgoingTradeBtn3");
        }

        public void OutgoingTradeBtn4()
        {
            NotifyAll("OutgoingTradeBtn4");
        }

        /*
         * Accept, Reject & Cancel
         */

        public void AcceptBtn()
        {
            NotifyAll("AcceptBtn");
        }

        public void RejectBtn()
        {
            NotifyAll("RejectBtn");
        }

        public void CancelBtn()
        {
            NotifyAll("CancelBtn");
        }


        private void NotifyAll(string key)
        {
            if (onButtonHit != null) onButtonHit(key);
        }
    }
}