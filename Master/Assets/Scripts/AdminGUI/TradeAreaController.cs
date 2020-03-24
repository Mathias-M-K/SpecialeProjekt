using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Container;
using CoreGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace AdminGUI
{
    public class TradeAreaController : MonoBehaviour, ITradeObserver
    {
        public int separationValue;
        public float animationSpeed;
        public int elementHeight;
        public LeanTweenType easeGlobal;
        public LeanTweenType easeIn;
        public LeanTweenType easeOut;
        public LeanTweenType moveIn;
        public LeanTweenType moveOut;

        [SerializeField] private int activeIncomingTrades = 0;
        private int activeOutgoingTrades = 0;

        private int outgoingTradeHeightOffSet = 0;

        //private List<PlayerTrade> incomingTrades = new List<PlayerTrade>();
        //private List<PlayerTrade> outgoingTrades = new List<PlayerTrade>();
        private List<Button> outgoingTradeButtons;
        private List<Button> incomingTradeButtons;
        private Dictionary<PlayerTrade, Button> incomingTradeDictionary = new Dictionary<PlayerTrade, Button>();
        private Dictionary<PlayerTrade, Button> outgoingTradeDictionary = new Dictionary<PlayerTrade, Button>();

        public TextMeshProUGUI incomingTradesTitle;
        public TextMeshProUGUI outgoingTradesTitle;

        public Button incomingTrade0;
        public Button incomingTrade1;
        public Button incomingTrade2;
        public Button incomingTrade3;

        public Button outgoingTrade1;
        public Button outgoingTrade2;
        public Button outgoingTrade3;
        public Button outgoingTrade4;

        private PlayerController _playerController;
        
        private void Start()
        {
            GUIEvents.current.onPlayerChange += OnPlayerChange;
            GUIEvents.current.onTradeAction += OnTradeBtn;
            incomingTradeButtons = new List<Button> {incomingTrade0, incomingTrade1, incomingTrade2, incomingTrade3};
            outgoingTradeButtons = new List<Button> {outgoingTrade1, outgoingTrade2, outgoingTrade3, outgoingTrade4};
        }

        private void Update()
        {
            if (easeGlobal != LeanTweenType.notUsed)
            {
                easeIn = easeGlobal;
                easeOut = easeGlobal;
                moveOut = easeGlobal;
            } 
        }


        private void OnTradeBtn(Button b, TradeActions action, Direction counterOffer)
        {
            PlayerTrade playerTrade = null;

            foreach (KeyValuePair<PlayerTrade,Button> valuePair in incomingTradeDictionary)
            {
                if (valuePair.Value == b)
                {
                    playerTrade = valuePair.Key;
                }
            }

            foreach (KeyValuePair<PlayerTrade,Button> valuePair in outgoingTradeDictionary)
            {
                if (valuePair.Value == b)
                {
                    playerTrade = valuePair.Key;
                }
            }

            if (playerTrade == null) throw new Exception("We got a problem bois...");


            switch (action)
            {
                case TradeActions.TradeRejected:
                    playerTrade.RejectTrade(_playerController);
                    break;
                case TradeActions.TradeAccepted:
                    playerTrade.AcceptTrade(counterOffer,_playerController);
                    break;
                case TradeActions.TradeCanceled:
                    playerTrade.CancelTrade(_playerController.player);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }

        private void OnPlayerChange(Player player)
        {
            if(_playerController != null)
            {
                _playerController.RemoveTradeObserver(this);
            }
            _playerController = GameHandler.current.GetPlayerController(player);
            _playerController.AddTradeObserver(this);
            
            StartCoroutine(UpdatePlayerInformation());
        }

        private IEnumerator  UpdatePlayerInformation()
        {
            if (activeIncomingTrades > 0 || activeOutgoingTrades > 0)
            {
                ClearTrades();
            
                yield return new WaitForSeconds(animationSpeed);
            }
            
            
            foreach (PlayerTrade trade in _playerController.GetIncomingTrades())
            {
                AddIncomingTrade(trade);
            }

            foreach (PlayerTrade trade in _playerController.GetOutgoingTrades())
            {
                AddOutgoingTrade(trade);
            }
        }

        private void AddIncomingTrade(PlayerTrade playerTrade)
        {
            //If no trades are active
            if (activeIncomingTrades == 0)
            {
                LeanTween.moveLocalX(incomingTradesTitle.gameObject, 0, animationSpeed).setEase(easeIn);
            }

            //Move already active trades
            for (int i = 0; i < activeIncomingTrades; i++)
            {
                Button btn = incomingTradeButtons[i];

                float newYPos = -300 + outgoingTradeHeightOffSet +
                                (elementHeight * (activeIncomingTrades - i)) +
                                (separationValue * (activeIncomingTrades - i));

                LeanTween.moveLocalY(btn.gameObject, newYPos, animationSpeed).setEase(moveIn);

                //Moving label the the correct spot
                if (i == 0)
                {
                    LeanTween.moveLocalY(incomingTradesTitle.gameObject, newYPos + elementHeight, animationSpeed).setEase(moveIn);
                }
            }

            //Getting btn to be added
            Button newBtn = incomingTradeButtons[activeIncomingTrades];
            incomingTradeDictionary.Add(playerTrade, newBtn);

            //Setting text on new btn
            TextMeshProUGUI t = newBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            t.text = $"{playerTrade.OfferingPlayer} | {playerTrade.DirectionOffer}";

            //Moving new Btn in
            LeanTween.moveLocalX(newBtn.gameObject, 0, animationSpeed).setEase(easeIn);
            LeanTween.moveLocalY(newBtn.gameObject, -300 + outgoingTradeHeightOffSet, animationSpeed).setEase(moveIn);
            
            //Adding one to active incoming trades
            //incomingTrades.Add(playerTrade);
            activeIncomingTrades++;
        }
        private void RemoveIncomingTrade(PlayerTrade playerTrade)
        {
            Button b = incomingTradeDictionary[playerTrade];
            incomingTradeDictionary.Remove(playerTrade);
            b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Deleted";

            incomingTradeButtons.Remove(b);
            incomingTradeButtons.Add(b);
            activeIncomingTrades--;

            float removeYPos = -300 + outgoingTradeHeightOffSet;
            LeanTween.moveLocalX(b.gameObject, -500, animationSpeed).setOnComplete(doStuff).setEase(easeOut);

            void doStuff()
            {
                LeanTween.moveLocalY(b.gameObject, removeYPos, animationSpeed).setEase(easeOut);
            }

            for (int i = 0; i < activeIncomingTrades; i++)
            {
                Button btn = incomingTradeButtons[i];
                float newYPos = -300 + outgoingTradeHeightOffSet + (elementHeight * (activeIncomingTrades - i-1)) + (separationValue * (activeIncomingTrades - i));

                LeanTween.moveLocalY(btn.gameObject, newYPos, animationSpeed).setEase(moveOut);
                if (i == 0)
                {
                    LeanTween.moveLocalY(incomingTradesTitle.gameObject, newYPos + elementHeight, animationSpeed).setEase(moveOut);
                }
            }

            if (activeIncomingTrades == 0)
            {
                LeanTween.moveLocalX(incomingTradesTitle.gameObject, -500, animationSpeed).setEase(easeOut);
            }
        }

        private void AddOutgoingTrade(PlayerTrade playerTrade)
        {
            if (activeOutgoingTrades == 0)
            {
                LeanTween.moveLocalX(outgoingTradesTitle.gameObject, 0, animationSpeed).setEase(easeIn);
                outgoingTradeHeightOffSet = elementHeight * 2 + separationValue;
            }
            else
            {
                outgoingTradeHeightOffSet += elementHeight + separationValue;
            }

            for (int i = 0; i < activeOutgoingTrades; i++)
            {
                Button btn = outgoingTradeButtons[i];

                float newYPos = -300  + (elementHeight * (activeOutgoingTrades - i)) + (separationValue * (activeOutgoingTrades - i));

                LeanTween.moveLocalY(btn.gameObject, newYPos, animationSpeed).setEase(moveIn);

                if (i == 0)
                {
                    LeanTween.moveLocalY(outgoingTradesTitle.gameObject, newYPos + elementHeight, animationSpeed).setEase(moveIn);
                    
                }
            }

            Button newBtn = outgoingTradeButtons[activeOutgoingTrades];
            outgoingTradeDictionary.Add(playerTrade, newBtn);

            TextMeshProUGUI t = newBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            t.text = $"{playerTrade.ReceivingPlayer} | {playerTrade.DirectionOffer}";

            LeanTween.moveLocalX(newBtn.gameObject, 0, animationSpeed).setEase(easeIn);
            LeanTween.moveLocalY(newBtn.gameObject, -300, animationSpeed).setEase(moveIn);
            
            activeOutgoingTrades++;
            //outgoingTrades.Add(playerTrade);

            OnOutgoingBtnChange(moveIn);
        }
        private void RemoveOutgoingTrade(PlayerTrade playerTrade)
        {
            outgoingTradeHeightOffSet -= elementHeight + separationValue;
            Button b = outgoingTradeDictionary[playerTrade];
            outgoingTradeDictionary.Remove(playerTrade);
            b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Deleted";

            outgoingTradeButtons.Remove(b);
            outgoingTradeButtons.Add(b);
            activeOutgoingTrades--;
            //outgoingTrades.Remove(playerTrade);

            float removeYPos = -300;
            LeanTween.moveLocalX(b.gameObject, -500, animationSpeed).setOnComplete(DelayedAction).setEase(easeOut);

            void DelayedAction()
            {
                LeanTween.moveLocalY(b.gameObject, removeYPos, animationSpeed).setEase(easeOut);
            }

            for (int i = 0; i < activeOutgoingTrades; i++)
            {
                Button btn = outgoingTradeButtons[i];
                float newYPos = -300 + (elementHeight * (activeOutgoingTrades - i-1)) + (separationValue * (activeOutgoingTrades - i));

                LeanTween.moveLocalY(btn.gameObject, newYPos, animationSpeed).setEase(moveOut);
                if (i == 0)
                {
                    LeanTween.moveLocalY(outgoingTradesTitle.gameObject, newYPos + elementHeight, animationSpeed).setEase(moveOut);
                }
            }

            if (activeOutgoingTrades == 0)
            {
                LeanTween.moveLocalX(outgoingTradesTitle.gameObject, -500, animationSpeed).setEase(easeOut);
                outgoingTradeHeightOffSet = 0;
            }

            OnOutgoingBtnChange(moveOut);
        }

        private void OnOutgoingBtnChange(LeanTweenType easeType)
        {
            //Moving all active incomingTrade values up
            for (int i = 0; i < activeIncomingTrades; i++)
            {
                Button btnToMove = incomingTradeButtons[i];

                float newYPos = -300 + outgoingTradeHeightOffSet - elementHeight +
                                (elementHeight * (activeIncomingTrades - i)) +
                                (separationValue * (activeIncomingTrades - i));
                
                LeanTween.moveLocalY(btnToMove.gameObject, newYPos, animationSpeed).setEase(easeType);

                if (i == 0)
                {
                    LeanTween.moveLocalY(incomingTradesTitle.gameObject, newYPos + elementHeight, animationSpeed).setEase(easeType);
                }
            }

            //Moving all inactive incomingTrade values up
            for (int i = activeIncomingTrades; i < 4; i++)
            {
                Button btnToMove = incomingTradeButtons[i];

                float newYPos = -300 + outgoingTradeHeightOffSet;
                LeanTween.moveLocalY(btnToMove.gameObject, newYPos, animationSpeed).setEase(moveOut);
                if (i == 0)
                {
                    LeanTween.moveLocalY(incomingTradesTitle.gameObject, newYPos + elementHeight, animationSpeed).setEase(moveOut);
                }
            }
        }

        private void ClearTrades()
        {
            Dictionary<PlayerTrade,Button> tempTradesIncoming = new Dictionary<PlayerTrade,Button>(incomingTradeDictionary);
            foreach (KeyValuePair<PlayerTrade,Button> valuePair in tempTradesIncoming)
            {
                RemoveIncomingTrade(valuePair.Key);
            }
            
            Dictionary<PlayerTrade,Button> tempTradesOutgoing = new Dictionary<PlayerTrade,Button>(outgoingTradeDictionary);
            foreach (KeyValuePair<PlayerTrade,Button> valuePair in tempTradesOutgoing)
            {
                RemoveOutgoingTrade(valuePair.Key);
            }
        }

        public void OnNewTradeActivity(PlayerTrade playerTrade, TradeActions tradeAction)
        {
            if (playerTrade.ReceivingPlayer == _playerController.player)
            {
                switch (tradeAction)
                {
                    case TradeActions.TradeOffered:
                        AddIncomingTrade(playerTrade);
                        break;
                    default:
                        RemoveIncomingTrade(playerTrade);
                        break;
                }
            }

            if (playerTrade.OfferingPlayer == _playerController.player)    
            {
                switch (tradeAction)
                {
                    case TradeActions.TradeOffered:
                        AddOutgoingTrade(playerTrade);
                        break;
                    default:
                        RemoveOutgoingTrade(playerTrade);
                        break;
                }
            }
        }
    }
}