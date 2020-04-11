using System.Collections.Generic;
using Container;
using CoreGame;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    public class OutgoingTradeListController : ParentTradeListController
    {
        protected override void OnTradeBtn(Button button, TradeActions tradeAction, Direction direction)
        {
            switch (tradeAction)
            {
                case TradeActions.TradeCanceled:
                    foreach (KeyValuePair<PlayerTrade,GameObject> element in ElementDictionary)
                    {
                        if (element.Key.TradeID == button.GetComponent<OutgoingTradeElement>().GetTradeId())
                        {
                            element.Key.CancelTrade(PlayerController.playerTag);
                            return;
                        }
                    }
                    break;
                case TradeActions.TradeCanceledByGameHandler:
                    foreach (KeyValuePair<PlayerTrade,GameObject> element in ElementDictionary)
                    {
                        if (element.Key.TradeID == button.GetComponent<IncomingTradeElement>().GetTradeId())
                        {
                            element.Key.CancelTrade(GameHandler.Current);
                            return;
                        }
                    }
                    break;
                default:
                    return;
            }
        }

        protected override void SetButtonValues(GameObject obj)
        {
            
        }

        public override void OnNewTradeActivity(PlayerTrade playerTrade, TradeActions tradeAction)
        {
            if (playerTrade.OfferingPlayerTags == PlayerController.playerTag)    
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