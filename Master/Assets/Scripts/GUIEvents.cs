using System;
using System.Security.Cryptography;
using CoreGame;
using DefaultNamespace;
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

        private PlayerTags _currentChosenPlayerTag = PlayerTags.Blank;
        
        private void Awake()
        {
            current = this;
        }

        public event Action<Button> OnButtonHit;
        public event Action<PlayerTags> onPlayerChange;
        public event Action<Button, TradeActions, Direction> OnTradeAction;
        public event Action OnManualOverride;
        public event Action OnGameStart;
        public event Action OnGameDone;
        public event Action <PlayerController> OnPlayerDone;


        //Yeha
        public event Action<bool, PlayerTags> OnPlayerReady;
        
        public void OnPlayerDoneNotify(PlayerController playerController)
        {
            if (OnPlayerDone != null) OnPlayerDone(playerController);
        }

        public void OnGameDoneNotify()
        {
            if (OnGameDone != null) OnGameDone();
        }
        
        /*
         * GAME BUTTONS
         */
        //Start game
        public void StartGame(Button b)
        {
            b.interactable = false;
            GameHandler.Current.StartGame();
            if (OnGameStart != null) OnGameStart();
            OnManualControl();
        }

        public void BtnHit(Button b)
        {
            NotifyButtonHit(b);
        }
        
        
        /*
         * im lazy, so here is another readyObserver... For the stattracker
         */
        public void OnPlayerReadyNotify(bool boolValue, PlayerTags player)
        {
            if (OnPlayerReady != null) OnPlayerReady(boolValue, player);
        }

        public void SetGameTag(PlayerTags playerTag)
        {
            _currentChosenPlayerTag = playerTag;
            GameHandler.Current.GetPlayerController(_currentChosenPlayerTag).AddReadyObserver(GameHandler.Current);
        }

        public PlayerTags GetCurrentPlayer()
        {
            return _currentChosenPlayerTag;
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
            if (OnTradeAction != null) OnTradeAction(b, action, counterOffer);
        }

        private void ManualEnabledNotify()
        {
            if (OnManualOverride != null) OnManualOverride();
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
            if (OnButtonHit != null) OnButtonHit(button);
        }
        
        private void OnPlayerChange()
        {
            if (_currentChosenPlayerTag == PlayerTags.Blank) return;
            if (onPlayerChange != null) onPlayerChange(_currentChosenPlayerTag);
        }
    }
}