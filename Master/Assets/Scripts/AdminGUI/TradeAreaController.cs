using System;
using System.Collections.Generic;
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

        [SerializeField] private int activeIncomingTrades = 0;
        private int activeOutgoingTrades = 0;

        private int outgoingTradeHeightOffSet = 0;

        private List<Button> outgoingTradeButtons;
        private List<Button> incomingTradeButtons;
        private Dictionary<PlayerTrade, Button> incomingTradeDictionary = new Dictionary<PlayerTrade, Button>();
        private Dictionary<PlayerTrade, Button> outgoingTradeDictionary = new Dictionary<PlayerTrade, Button>();

        public TextMeshProUGUI incomingTradesTitle;
        public TextMeshProUGUI outgoingTradesTitle;

        public Button incomingTrade1;
        public Button incomingTrade2;
        public Button incomingTrade3;
        public Button incomingTrade4;

        public Button outgoingTrade1;
        public Button outgoingTrade2;
        public Button outgoingTrade3;
        public Button outgoingTrade4;

        private PlayerController _playerController;

        private PlayerTrade p1;
        private PlayerTrade p2;
        private PlayerTrade p3;
        private PlayerTrade p4;


        private void Start()
        {
            GUIEvents.current.onPlayerChange += OnPlayerChanged;
            incomingTradeButtons = new List<Button> {incomingTrade1, incomingTrade2, incomingTrade3, incomingTrade4};
            outgoingTradeButtons = new List<Button> {outgoingTrade1, outgoingTrade2, outgoingTrade3, outgoingTrade4};


            p1 = new PlayerTrade(Player.Red, Player.Blue, Direction.Up, GameHandler.current, 2, null);
            p2 = new PlayerTrade(Player.Red, Player.Green, Direction.Down, GameHandler.current, 2, null);
            p3 = new PlayerTrade(Player.Blue, Player.Yellow, Direction.Left, GameHandler.current, 2, null);
            p4 = new PlayerTrade(Player.Blue, Player.Red, Direction.Right, GameHandler.current, 2, null);
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.I) && Input.GetKeyDown(KeyCode.Alpha1)) AddIncomingTrade(p1);
            if (Input.GetKey(KeyCode.I) && Input.GetKeyDown(KeyCode.Alpha2)) AddIncomingTrade(p2);
            if (Input.GetKey(KeyCode.I) && Input.GetKeyDown(KeyCode.Alpha3)) AddIncomingTrade(p3);
            if (Input.GetKey(KeyCode.I) && Input.GetKeyDown(KeyCode.Alpha4)) AddIncomingTrade(p4);
            
            if (Input.GetKey(KeyCode.O) && Input.GetKeyDown(KeyCode.Alpha1)) AddOutgoingTrade(p1);
            if (Input.GetKey(KeyCode.O) && Input.GetKeyDown(KeyCode.Alpha2)) AddOutgoingTrade(p2);
            if (Input.GetKey(KeyCode.O) && Input.GetKeyDown(KeyCode.Alpha3)) AddOutgoingTrade(p3);
            if (Input.GetKey(KeyCode.O) && Input.GetKeyDown(KeyCode.Alpha4)) AddOutgoingTrade(p4);

            if (Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.Alpha1)) RemoveIncomingTrade(p1);
            if (Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.Alpha2)) RemoveIncomingTrade(p2);
            if (Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.Alpha3)) RemoveIncomingTrade(p3);
            if (Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.Alpha4)) RemoveIncomingTrade(p4);
            
            if (Input.GetKey(KeyCode.F) && Input.GetKeyDown(KeyCode.Alpha1)) RemoveOutgoingTrade(p1);
            if (Input.GetKey(KeyCode.F) && Input.GetKeyDown(KeyCode.Alpha2)) RemoveOutgoingTrade(p2);
            if (Input.GetKey(KeyCode.F) && Input.GetKeyDown(KeyCode.Alpha3)) RemoveOutgoingTrade(p3);
            if (Input.GetKey(KeyCode.F) && Input.GetKeyDown(KeyCode.Alpha4)) RemoveOutgoingTrade(p4);
        }

        private void OnPlayerChanged(Player player)
        {
            if(_playerController != null)
            {
                _playerController.RemoveTradeObserver(this);
                _playerController = GameHandler.current.GetPlayerController(player);
            }
            _playerController = GameHandler.current.GetPlayerController(player);
            _playerController.AddTradeObserver(this);
            
            UpdatePlayerInformation();
        }

        private void UpdatePlayerInformation()
        {
            foreach (PlayerTrade trade in _playerController.GetIncomingTrades())
            {
                AddIncomingTrade(trade);
            }
        }

        private void AddIncomingTrade(PlayerTrade playerTrade)
        {
            //If no trades are active
            if (activeIncomingTrades == 0)
            {
                LeanTween.moveLocalX(incomingTradesTitle.gameObject, 0, animationSpeed);
            }

            //Move already active trades
            for (int i = 0; i < activeIncomingTrades; i++)
            {
                Button btn = incomingTradeButtons[i];

                float newYPos = -300 + outgoingTradeHeightOffSet +
                                (elementHeight * (activeIncomingTrades - i)) +
                                (separationValue * (activeIncomingTrades - i));

                LeanTween.moveLocalY(btn.gameObject, newYPos, animationSpeed);

                //Moving label the the correct spot
                if (i == 0)
                {
                    LeanTween.moveLocalY(incomingTradesTitle.gameObject, newYPos + elementHeight, animationSpeed);
                }
            }

            //Getting btn to be added
            Button newBtn = incomingTradeButtons[activeIncomingTrades];
            incomingTradeDictionary.Add(playerTrade, newBtn);

            //Setting text on new btn
            TextMeshProUGUI t = newBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            t.text = $"{playerTrade.OfferingPlayer} | {playerTrade.DirectionOffer}";

            //Moving new Btn in
            LeanTween.moveLocalX(newBtn.gameObject, 0, animationSpeed);
            LeanTween.moveLocalY(newBtn.gameObject, -300 + outgoingTradeHeightOffSet, animationSpeed);
            
            //Adding one to active incoming trades
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
            LeanTween.moveLocalX(b.gameObject, -500, animationSpeed).setOnComplete(doStuff);

            void doStuff()
            {
                LeanTween.moveLocalY(b.gameObject, removeYPos, animationSpeed);
            }

            for (int i = 0; i < activeIncomingTrades; i++)
            {
                Button btn = incomingTradeButtons[i];
                float newYPos = -300 + outgoingTradeHeightOffSet + (elementHeight * (activeIncomingTrades - i-1)) + (separationValue * (activeIncomingTrades - i));

                LeanTween.moveLocalY(btn.gameObject, newYPos, animationSpeed);
                if (i == 0)
                {
                    LeanTween.moveLocalY(incomingTradesTitle.gameObject, newYPos + elementHeight, animationSpeed);
                }
            }

            if (activeIncomingTrades == 0)
            {
                LeanTween.moveLocalX(incomingTradesTitle.gameObject, -500, animationSpeed);
            }
        }

        private void AddOutgoingTrade(PlayerTrade playerTrade)
        {
            if (activeOutgoingTrades == 0)
            {
                LeanTween.moveLocalX(outgoingTradesTitle.gameObject, 0, animationSpeed);
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

                LeanTween.moveLocalY(btn.gameObject, newYPos, animationSpeed);

                if (i == 0)
                {
                    LeanTween.moveLocalY(outgoingTradesTitle.gameObject, newYPos + elementHeight, animationSpeed);
                    
                }
            }

            Button newBtn = outgoingTradeButtons[activeOutgoingTrades];
            outgoingTradeDictionary.Add(playerTrade, newBtn);

            TextMeshProUGUI t = newBtn.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            t.text = $"{playerTrade.OfferingPlayer} | {playerTrade.DirectionOffer}";

            LeanTween.moveLocalX(newBtn.gameObject, 0, animationSpeed);
            LeanTween.moveLocalY(newBtn.gameObject, -300, animationSpeed);
            
            activeOutgoingTrades++;

            OnOutgoingBtnChange();
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

            float removeYPos = -300;
            LeanTween.moveLocalX(b.gameObject, -500, animationSpeed).setOnComplete(DelayedAction);

            void DelayedAction()
            {
                LeanTween.moveLocalY(b.gameObject, removeYPos, animationSpeed);
            }

            for (int i = 0; i < activeOutgoingTrades; i++)
            {
                Button btn = outgoingTradeButtons[i];
                float newYPos = -300 + (elementHeight * (activeOutgoingTrades - i-1)) + (separationValue * (activeOutgoingTrades - i));

                LeanTween.moveLocalY(btn.gameObject, newYPos, animationSpeed);
                if (i == 0)
                {
                    LeanTween.moveLocalY(outgoingTradesTitle.gameObject, newYPos + elementHeight, animationSpeed);
                }
            }

            if (activeOutgoingTrades == 0)
            {
                LeanTween.moveLocalX(outgoingTradesTitle.gameObject, -500, animationSpeed);
                outgoingTradeHeightOffSet = 0;
            }

            OnOutgoingBtnChange();
        }

        private void OnOutgoingBtnChange()
        {
            //Moving all active incomingTrade values up
            for (int i = 0; i < activeIncomingTrades; i++)
            {
                Button btnToMove = incomingTradeButtons[i];

                float newYPos = -300 + outgoingTradeHeightOffSet - elementHeight +
                                (elementHeight * (activeIncomingTrades - i)) +
                                (separationValue * (activeIncomingTrades - i));
                LeanTween.moveLocalY(btnToMove.gameObject, newYPos, animationSpeed);

                if (i == 0)
                {
                    LeanTween.moveLocalY(incomingTradesTitle.gameObject, newYPos + elementHeight, animationSpeed);
                }
            }

            //Moving all inactive incomingTrade values up
            for (int i = activeIncomingTrades; i < 4; i++)
            {
                Button btnToMove = incomingTradeButtons[i];

                float newYPos = -300 + outgoingTradeHeightOffSet;
                LeanTween.moveLocalY(btnToMove.gameObject, newYPos, animationSpeed);
                if (i == 0)
                {
                    LeanTween.moveLocalY(incomingTradesTitle.gameObject, newYPos + elementHeight, animationSpeed);
                }
            }
        }

        public void TradeUpdate(PlayerTrade playerTrade, TradeActions tradeAction)
        {
            throw new NotImplementedException();
        }
    }
}