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
        public Player currentChosenPlayer;
        
        //Colors
        public Color32 red = new Color32(229, 80, 57,255);
        private Color32 blue = new Color32(74, 105, 189,255);
        private Color32 green = new Color32(120, 224, 143,255);
        private Color32 yellow = new Color32(246, 185, 59,255);

        private void Awake()
        {
            current = this;
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
                    playerDropdown.image.color = red;
                    break;
                case 1:
                    currentChosenPlayer = Player.Blue;
                    playerDropdown.image.color = blue;
                    break;
                case 2:
                    currentChosenPlayer = Player.Green;
                    playerDropdown.image.color = green;
                    break;
                case 3:
                    currentChosenPlayer = Player.Yellow;
                    playerDropdown.image.color = yellow;
                    break;
            }
            
            if (onPlayerChange != null) onPlayerChange(currentChosenPlayer);
        }

        public Color32 GetPlayerColor(Player p)
        {
            switch (p)
            {
                case Player.Red:
                    return red;
                case Player.Blue:
                    return blue;
                case Player.Green:
                    return green;
                case Player.Yellow:
                    return yellow;
                default:
                    throw new ArgumentOutOfRangeException(nameof(p), p, null);
            }
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

        public void AcceptBtn()
        {
            NotifyAll("AcceptBtn");
        }

        public void RejectBtn()
        {
            NotifyAll("RejectBtn");
        }


        private void NotifyAll(string key)
        {
            if (onButtonHit != null) onButtonHit(key);
        }
    }
}