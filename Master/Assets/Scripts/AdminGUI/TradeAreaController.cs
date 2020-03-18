using System;
using System.Collections.Generic;
using Container;
using CoreGame;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdminGUI
{
    public class TradeAreaController : MonoBehaviour
    {
        public int separationValue;
        public int elementHeight;

        private int activeIncomingTrades = 0;
        private int activeOutgoingTrades = 0;

        private int outgoingTradeHeightOffSet = 0;

        public TextMeshProUGUI incomingTradesTitle;
        private List<Button> incomingTradeButtons;
        public Button incomingTrade1;
        public Button incomingTrade2;
        public Button incomingTrade3;
        public Button incomingTrade4;

        public TextMeshProUGUI outgoingTradesTitle;
        private List<Button> outgoingTradeButtons;
        public Button outgoingTrade1;
        public Button outgoingTrade2;
        public Button outgoingTrade3;
        public Button outgoingTrade4;

        private PlayerTrade p1;
        private PlayerTrade p2;
        private PlayerTrade p3;
        private PlayerTrade p4;


        private void Start()
        {
            incomingTradeButtons = new List<Button> {incomingTrade1, incomingTrade2, incomingTrade3, incomingTrade4};
            outgoingTradeButtons = new List<Button> {outgoingTrade1, outgoingTrade2, outgoingTrade3, outgoingTrade4};

            

            p1 = new PlayerTrade(Player.Red, Player.Blue, Direction.Up, GameHandler.current, 2, null);
            p2 = new PlayerTrade(Player.Red, Player.Green, Direction.Down, GameHandler.current, 2, null);
            p3 = new PlayerTrade(Player.Blue, Player.Yellow, Direction.Left, GameHandler.current, 2, null);
            p4 = new PlayerTrade(Player.Blue, Player.Red, Direction.Right, GameHandler.current, 2, null);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                AddIncomingTrade(p1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                AddIncomingTrade(p2);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                AddOutgoingTrade(p3);
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                AddOutgoingTrade(p4);
            }
        }

        private void AddIncomingTrade(PlayerTrade playerTrade)
        {
            if (activeIncomingTrades == 0)
            {
                Button btnToMove = incomingTradeButtons[activeIncomingTrades];
                btnToMove.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{playerTrade.OfferingPlayer} | {playerTrade.DirectionOffer}";

                LeanTween.moveLocalX(incomingTradesTitle.gameObject, 0, 1);
                LeanTween.moveLocalX(btnToMove.gameObject, 0, 1);
                activeIncomingTrades++;
            }
            else
            {
                for (int i = 0; i < activeIncomingTrades; i++)
                {
                    Button btnToMove = incomingTradeButtons[i];

                    float newYPos = -300+outgoingTradeHeightOffSet + (elementHeight*(activeIncomingTrades-i)) + (separationValue*(activeIncomingTrades-i));
                    LeanTween.moveLocalY(btnToMove.gameObject, newYPos, 1);

                    if (i == 0)
                    {
                        LeanTween.moveLocalY(incomingTradesTitle.gameObject, newYPos + elementHeight, 1);
                    }
                }

                Button btnToAdd = incomingTradeButtons[activeIncomingTrades];
                btnToAdd.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{playerTrade.OfferingPlayer} | {playerTrade.DirectionOffer}";
                LeanTween.moveLocalX(btnToAdd.gameObject, 0, 1);
                LeanTween.moveLocalY(btnToAdd.gameObject, -300+outgoingTradeHeightOffSet, 1);

                activeIncomingTrades++;
            }
        }

        private void AddOutgoingTrade(PlayerTrade playerTrade)
        {
            if (activeOutgoingTrades == 0)
            {
                outgoingTradeHeightOffSet = elementHeight*2+separationValue;
                
                Button btnToMove = outgoingTradeButtons[activeOutgoingTrades];

                btnToMove.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{playerTrade.ReceivingPlayer} | {playerTrade.DirectionOffer}";
                LeanTween.moveLocalX(outgoingTradesTitle.gameObject, 0, 1);
                LeanTween.moveLocalX(btnToMove.gameObject, 0, 1);
                activeOutgoingTrades++;
            }
            else
            {
                outgoingTradeHeightOffSet += elementHeight+separationValue;
                
                for (int i = 0; i < activeOutgoingTrades; i++)
                {
                    Button btnToMove = outgoingTradeButtons[i];

                    float newYPos = -300 + (elementHeight*(activeOutgoingTrades-i)) + (separationValue*(activeOutgoingTrades-i));
                    LeanTween.moveLocalY(btnToMove.gameObject, newYPos, 1);

                    if (i == 0)
                    {
                        LeanTween.moveLocalY(outgoingTradesTitle.gameObject, newYPos + elementHeight, 1);
                    }
                }

                Button btnToAdd = outgoingTradeButtons[activeOutgoingTrades];
                btnToAdd.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{playerTrade.OfferingPlayer} | {playerTrade.DirectionOffer}";
                LeanTween.moveLocalX(btnToAdd.gameObject, 0, 1);
                LeanTween.moveLocalY(btnToAdd.gameObject, -300, 1);

                activeOutgoingTrades++;
            }
            
            //Moving all active incomingTrade values up
            for (int i = 0; i < activeIncomingTrades; i++)
            {
                Button btnToMove = incomingTradeButtons[i];

                float newYPos = -300+outgoingTradeHeightOffSet-elementHeight + (elementHeight*(activeIncomingTrades-i)) + (separationValue*(activeIncomingTrades-i));
                LeanTween.moveLocalY(btnToMove.gameObject, newYPos, 1);

                if (i == 0)
                {
                    LeanTween.moveLocalY(incomingTradesTitle.gameObject, newYPos + elementHeight, 1);
                }
            }
            
            //Moving all inactive incomingTrade values up
            for (int i = activeIncomingTrades; i < 4; i++)
            {
                Button btnToMove = incomingTradeButtons[i];

                float newYPos = -300+outgoingTradeHeightOffSet;
                LeanTween.moveLocalY(btnToMove.gameObject, newYPos, 1);
            }
        }
        
    }
}