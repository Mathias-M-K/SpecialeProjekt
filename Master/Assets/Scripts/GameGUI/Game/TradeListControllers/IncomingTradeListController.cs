using System;
using System.Collections.Generic;
using Container;
using CoreGame;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    public class IncomingTradeListController : ParentTradeListController
    {
        protected override void OnTradeBtn(Button button, TradeActions tradeAction, Direction direction)
        {
            switch (tradeAction)
            {
                case TradeActions.TradeRejected:
                    foreach (KeyValuePair<PlayerTrade,GameObject> element in ElementDictionary)
                    {
                        if (element.Key.TradeID == button.GetComponent<IncomingTradeElement>().GetTradeId())
                        {
                            element.Key.RejectTrade(PlayerController);
                            return;
                        }
                    }
                    
                    break;
                case TradeActions.TradeAccepted:
                    foreach (KeyValuePair<PlayerTrade,GameObject> element in ElementDictionary)
                    {
                        if (element.Key.TradeID == button.GetComponent<IncomingTradeElement>().GetTradeId())
                        {
                            element.Key.AcceptTrade(direction, PlayerController);
                            return;
                        }
                    }
                    break;
                default:
                    return;
            }
        }
        

        public override void OnNewTradeActivity(PlayerTrade playerTrade, TradeActions tradeAction)
        {
            if (playerTrade.ReceivingPlayerTags == PlayerController.playerTag)
            {
                switch (tradeAction)
                {
                    case TradeActions.TradeOffered:
                        AddElement(playerTrade);
                        break;
                    default:
                        RemoveElement(playerTrade);
                        break;
                }
            }
        }
    }
}