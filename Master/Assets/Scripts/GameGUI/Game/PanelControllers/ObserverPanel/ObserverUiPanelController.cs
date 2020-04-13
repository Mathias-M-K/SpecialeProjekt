using System;
using System.Collections.Generic;
using System.Linq;
using Container;
using CoreGame;
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
        
        private List<PlayerTrade> _trades = new List<PlayerTrade>();
        private Dictionary<PlayerTrade,GameObject> _tradeDictionary = new Dictionary<PlayerTrade, GameObject>();

        private void Start()
        {
            GUIEvents.current.OnManualOverride += OnManualOverride;
            GameHandler.Current.AddTradeObserver(this);
        }

        private void OnManualOverride()
        {
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
            
            go.GetComponent<ObserverTradeElement>().RemoveTrade(tradeAction);
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