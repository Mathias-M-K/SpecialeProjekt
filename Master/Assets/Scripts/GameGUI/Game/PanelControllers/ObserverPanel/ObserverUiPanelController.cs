using System;
using System.Collections.Generic;
using System.Linq;
using Container;
using CoreGame;
using DefaultNamespace;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace AdminGUI.PanelControllers
{
    public class ObserverUiPanelController : MonoBehaviour, ITradeObserver
    {
        [Header("Rects")]
        public GameObject tradeElementsRect;
        public GameObject playerStatsRect;

        [Header("Prefabs")] 
        public GameObject tradePrefab;
        public GameObject playerStatPrefab;

        [Header("Other")] 
        public TextMeshProUGUI timer;
        public NetworkAgentController netAgentController;
        
        private List<PlayerTrade> _trades = new List<PlayerTrade>();
        private Dictionary<PlayerTrade,GameObject> _tradeDictionary = new Dictionary<PlayerTrade, GameObject>();

        private float _startTime;

        private void Start()
        {
            GUIEvents.current.OnManualOverride += OnManualOverride;
            GameHandler.Current.AddTradeObserver(this);
        }

        private void Update()
        {
            TimeSpan ts = TimeSpan.FromSeconds(Time.realtimeSinceStartup-_startTime);

            //timer.text = $"{ts.Hours}:{ts.Minutes}:{ts.Seconds}:{ts.Milliseconds.ToString("##")}";
            timer.text = $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}";
        }

        public void ObserverBtnPressed()
        {
            netAgentController.OnObserverMark(PhotonNetwork.NickName,GetTime());
        }

        private float GetTime()
        {
            return Time.realtimeSinceStartup - _startTime;
        }

        private void OnManualOverride()
        {
            _startTime = Time.realtimeSinceStartup;
            CreatePlayerStatPanels();
        }
        
        public void OnNewTradeActivity(PlayerTrade playerTrade, TradeActions tradeAction)
        {
            if (tradeAction == TradeActions.TradeOffered)
            {
                AddTradeElement(playerTrade);
            }
            else
            {
                RemoveTradeElement(playerTrade, tradeAction);
            }
        }

        private void AddTradeElement(PlayerTrade trade)
        {
            if (_trades.Any(item => item.TradeID == trade.TradeID)) return;
            
            GameObject go = Instantiate(tradePrefab, tradeElementsRect.transform, false);
            _trades.Add(trade);
            _tradeDictionary.Add(trade,go);
            
            ObserverTradeElement ote = go.GetComponent<ObserverTradeElement>();
            
            ote.SetInfo(trade);
        }

        private void RemoveTradeElement(PlayerTrade trade, TradeActions tradeAction)
        {
            if (_trades.All(item => item.TradeID != trade.TradeID)) return;

            _trades.Remove(trade);
            GameObject go = _tradeDictionary[trade];
            _tradeDictionary.Remove(trade);
            
            go.GetComponent<ObserverTradeElement>().RemoveTrade(tradeAction,trade);
        }

        private void CreatePlayerStatPanels()
        {
            foreach (PlayerController controller in GameHandler.Current.GetPlayers())
            {
                GameObject go = Instantiate(playerStatPrefab, playerStatsRect.transform, false);
                ObserverPlayerStatElement ote = go.GetComponent<ObserverPlayerStatElement>();
                
                ote.SetPlayerController(controller);

            }
        }

        
    }
}