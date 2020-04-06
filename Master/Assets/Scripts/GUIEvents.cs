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

        private PlayerTags _currentChosenPlayerTags;
        
        private void Awake()
        {
            current = this;
        }

        public event Action<Button> onButtonHit;
        public event Action<PlayerTags> onPlayerChange;
        public event Action<Button, TradeActions, Direction> onTradeAction;
        public event Action onManualOverride;
        public event Action onGameStart;
        
        /*
         * GAME BUTTONS
         */
        //Start game
        public void StartGame()
        {
            _currentChosenPlayerTags = PlayerTags.Red;
            GameHandler.current.StartGame();
            if (onGameStart != null) onGameStart();
        }

        public void BtnHit(Button b)
        {
            NotifyButtonHit(b);
        }

        /*
         * ADMIN GUI BUTTONS
         */
        //Manual Control Button
        public void OnManualControl()
        {
            OnPlayerChange();
            ManualEnabledNotify();
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

        private void NotifyButtonHit()
        {
            GameObject go = new GameObject("FakeGameObject");
            Button b = go.AddComponent<Button>();
            
            Destroy(go,1);
            
            NotifyButtonHit(b);
        }
        private void NotifyButtonHit(Button button)
        {
            if (onButtonHit != null) onButtonHit(button);
        }

        private void OnPlayerChange()
        {
            if (onPlayerChange != null) onPlayerChange(_currentChosenPlayerTags);
        }
    }
}