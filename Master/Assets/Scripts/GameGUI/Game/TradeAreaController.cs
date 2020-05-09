using System;
using System.Collections;
using System.Collections.Generic;
using Container;
using CoreGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

        private int activeIncomingTrades = 0;
        private int _activeOutgoingTrades = 0;
        private int _outgoingTradeHeightOffSet = 0;

        private List<PlayerTrade> incomingTrades = new List<PlayerTrade>();
        private List<PlayerTrade> outgoingTrades = new List<PlayerTrade>();
        private List<Button> _outgoingTradeButtons = new List<Button>();
        private List<Button> _incomingTradeButtons = new List<Button>();
        private readonly Dictionary<PlayerTrade, Button> _incomingTradeDictionary = new Dictionary<PlayerTrade, Button>();
        private readonly Dictionary<PlayerTrade, Button> _outgoingTradeDictionary = new Dictionary<PlayerTrade, Button>();

        public TextMeshProUGUI incomingTradesTitle;
        public TextMeshProUGUI outgoingTradesTitle;

        /*public Button incomingTrade0;
        public Button incomingTrade1;
        public Button incomingTrade2;
        public Button incomingTrade3;

        public Button outgoingTrade1;
        public Button outgoingTrade2;
        public Button outgoingTrade3;
        public Button outgoingTrade4;*/

        private PlayerController _playerController;
        
        private void Start()
        {
            //GUIEvents.current.onPlayerChange += OnPlayerChange;
            //GUIEvents.current.OnTradeAction += OnTradeBtn;
            //_incomingTradeButtons = new List<Button> {incomingTrade0, incomingTrade1, incomingTrade2, incomingTrade3};
            //_outgoingTradeButtons = new List<Button> {outgoingTrade1, outgoingTrade2, outgoingTrade3, outgoingTrade4};
        }

        private void Update()
        {
            if (easeGlobal != LeanTweenType.notUsed)
            {
                easeIn = easeGlobal;
                easeOut = easeGlobal;
                moveOut = easeGlobal;
            }

            /*if (Input.GetKey(KeyCode.I) && Input.GetKeyDown(KeyCode.A))
            {
                int randomOfferingPlayerNr = Random.Range(1, 16);
                int randomReceivingPlayerNr = Random.Range(1, 16);
                int randomMoveNr = Random.Range(1, 4);

                PlayerTags randomOfferingPlayer = GlobalMethods.GetTagByNumber(randomOfferingPlayerNr);
                PlayerTags randomReceivingPlayer = GlobalMethods.GetTagByNumber(randomReceivingPlayerNr);

                Direction direction;
                switch (randomMoveNr)
                {
                    case 1:
                        direction = Direction.Right;
                        break;
                    case 2:
                        direction = Direction.Left;
                        break;
                    case 3:
                        direction = Direction.Up;
                        break;
                    case 4:
                        direction = Direction.Down;
                        break;
                    default:
                        direction = Direction.Right;
                        break;
                }

                PlayerTrade trade = new PlayerTrade(randomOfferingPlayer, randomReceivingPlayer, direction, null, 2,
                    null, 2);

                AddIncomingTrade(trade);
            }*/
        }


        private void OnTradeBtn(Button b, TradeActions action, Direction counterOffer)
        {
            PlayerTrade playerTrade = null;

            foreach (KeyValuePair<PlayerTrade,Button> valuePair in _incomingTradeDictionary)
            {
                if (valuePair.Value == b)
                {
                    playerTrade = valuePair.Key;
                }
            }

            foreach (KeyValuePair<PlayerTrade,Button> valuePair in _outgoingTradeDictionary)
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
                    playerTrade.CancelTrade(_playerController.playerTag);
                    break;
                case TradeActions.TradeCanceledByGameHandler:
                    playerTrade.CancelTrade(GameHandler.Current);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }

        private void OnPlayerChange(PlayerTags playerTags)
        {
            if(_playerController != null)
            {
                _playerController.RemoveTradeObserver(this);
            }
            _playerController = GameHandler.Current.GetPlayerController(playerTags);
            _playerController.AddTradeObserver(this);
            
            StartCoroutine(UpdatePlayerInformation());
        }

        private IEnumerator  UpdatePlayerInformation()
        {
            if (activeIncomingTrades > 0 || _activeOutgoingTrades > 0)
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
                Button btn = _incomingTradeButtons[i];

                float newYPos = -300 + _outgoingTradeHeightOffSet +
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
            Button newBtn = _incomingTradeButtons[activeIncomingTrades];
            _incomingTradeDictionary.Add(playerTrade, newBtn);

            //Setting text on new btn
            TextMeshProUGUI t = newBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            t.text = $"{playerTrade.OfferingPlayerTags} | {playerTrade.DirectionOffer}";

            //Moving new Btn in
            LeanTween.moveLocalX(newBtn.gameObject, 0, animationSpeed).setEase(easeIn);
            LeanTween.moveLocalY(newBtn.gameObject, -300 + _outgoingTradeHeightOffSet, animationSpeed).setEase(moveIn);
            
            //Adding one to active incoming trades
            //incomingTrades.Add(playerTrade);
            activeIncomingTrades++;
        }
        private void RemoveIncomingTrade(PlayerTrade playerTrade)
        {
            Button b = _incomingTradeDictionary[playerTrade];
            _incomingTradeDictionary.Remove(playerTrade);
            b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Deleted";

            _incomingTradeButtons.Remove(b);
            _incomingTradeButtons.Add(b);
            activeIncomingTrades--;

            float removeYPos = -300 + _outgoingTradeHeightOffSet;
            LeanTween.moveLocalX(b.gameObject, -500, animationSpeed).setOnComplete(doStuff).setEase(easeOut);

            void doStuff()
            {
                LeanTween.moveLocalY(b.gameObject, removeYPos, animationSpeed).setEase(easeOut);
            }

            for (int i = 0; i < activeIncomingTrades; i++)
            {
                Button btn = _incomingTradeButtons[i];
                float newYPos = -300 + _outgoingTradeHeightOffSet + (elementHeight * (activeIncomingTrades - i-1)) + (separationValue * (activeIncomingTrades - i));

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
            if (_activeOutgoingTrades == 0)
            {
                LeanTween.moveLocalX(outgoingTradesTitle.gameObject, 0, animationSpeed).setEase(easeIn);
                _outgoingTradeHeightOffSet = elementHeight * 2 + separationValue;
            }
            else
            {
                _outgoingTradeHeightOffSet += elementHeight + separationValue;
            }

            for (int i = 0; i < _activeOutgoingTrades; i++)
            {
                Button btn = _outgoingTradeButtons[i];

                float newYPos = -300  + (elementHeight * (_activeOutgoingTrades - i)) + (separationValue * (_activeOutgoingTrades - i));

                LeanTween.moveLocalY(btn.gameObject, newYPos, animationSpeed).setEase(moveIn);

                if (i == 0)
                {
                    LeanTween.moveLocalY(outgoingTradesTitle.gameObject, newYPos + elementHeight, animationSpeed).setEase(moveIn);
                    
                }
            }

            Button newBtn = _outgoingTradeButtons[_activeOutgoingTrades];
            _outgoingTradeDictionary.Add(playerTrade, newBtn);

            TextMeshProUGUI t = newBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            t.text = $"{playerTrade.ReceivingPlayerTags} | {playerTrade.DirectionOffer}";

            LeanTween.moveLocalX(newBtn.gameObject, 0, animationSpeed).setEase(easeIn);
            LeanTween.moveLocalY(newBtn.gameObject, -300, animationSpeed).setEase(moveIn);
            
            _activeOutgoingTrades++;
            //outgoingTrades.Add(playerTrade);

            OnOutgoingBtnChange(moveIn);
        }
        private void RemoveOutgoingTrade(PlayerTrade playerTrade)
        {
            _outgoingTradeHeightOffSet -= elementHeight + separationValue;
            Button b = _outgoingTradeDictionary[playerTrade];
            _outgoingTradeDictionary.Remove(playerTrade);
            b.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Deleted";

            _outgoingTradeButtons.Remove(b);
            _outgoingTradeButtons.Add(b);
            _activeOutgoingTrades--;
            //outgoingTrades.Remove(playerTrade);

            float removeYPos = -300;
            LeanTween.moveLocalX(b.gameObject, -500, animationSpeed).setOnComplete(DelayedAction).setEase(easeOut);

            void DelayedAction()
            {
                LeanTween.moveLocalY(b.gameObject, removeYPos, animationSpeed).setEase(easeOut);
            }

            for (int i = 0; i < _activeOutgoingTrades; i++)
            {
                Button btn = _outgoingTradeButtons[i];
                float newYPos = -300 + (elementHeight * (_activeOutgoingTrades - i-1)) + (separationValue * (_activeOutgoingTrades - i));

                LeanTween.moveLocalY(btn.gameObject, newYPos, animationSpeed).setEase(moveOut);
                if (i == 0)
                {
                    LeanTween.moveLocalY(outgoingTradesTitle.gameObject, newYPos + elementHeight, animationSpeed).setEase(moveOut);
                }
            }

            if (_activeOutgoingTrades == 0)
            {
                LeanTween.moveLocalX(outgoingTradesTitle.gameObject, -500, animationSpeed).setEase(easeOut);
                _outgoingTradeHeightOffSet = 0;
            }

            OnOutgoingBtnChange(moveOut);
        }

        private void OnOutgoingBtnChange(LeanTweenType easeType)
        {
            //Moving all active incomingTrade values up
            for (int i = 0; i < activeIncomingTrades; i++)
            {
                Button btnToMove = _incomingTradeButtons[i];

                float newYPos = -300 + _outgoingTradeHeightOffSet - elementHeight +
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
                Button btnToMove = _incomingTradeButtons[i];

                float newYPos = -300 + _outgoingTradeHeightOffSet;
                LeanTween.moveLocalY(btnToMove.gameObject, newYPos, animationSpeed).setEase(moveOut);
                if (i == 0)
                {
                    LeanTween.moveLocalY(incomingTradesTitle.gameObject, newYPos + elementHeight, animationSpeed).setEase(moveOut);
                }
            }
        }

        private void ClearTrades()
        {
            Dictionary<PlayerTrade,Button> tempTradesIncoming = new Dictionary<PlayerTrade,Button>(_incomingTradeDictionary);
            foreach (KeyValuePair<PlayerTrade,Button> valuePair in tempTradesIncoming)
            {
                RemoveIncomingTrade(valuePair.Key);
            }
            
            Dictionary<PlayerTrade,Button> tempTradesOutgoing = new Dictionary<PlayerTrade,Button>(_outgoingTradeDictionary);
            foreach (KeyValuePair<PlayerTrade,Button> valuePair in tempTradesOutgoing)
            {
                RemoveOutgoingTrade(valuePair.Key);
            }
        }

        public void OnNewTradeActivity(PlayerTrade playerTrade, TradeActions tradeAction)
        {
            if (playerTrade.ReceivingPlayerTags == _playerController.playerTag)
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

            if (playerTrade.OfferingPlayerTags == _playerController.playerTag)    
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