
using System;
using System.Collections.Generic;
using Container;
using CoreGame;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AdminGUI
{
    public class ExternalPlayerControl : MonoBehaviour,ITradeObserver
    {
        [Header("Set Player")] public Player player;
        [Header("Set Game Handler")] public GameHandler gameHandler;
        [SerializeField]private PlayerController _playerController;
        public List<PlayerTrade> trades;
        
        [Header("Direction Buttons")]public Button left1;
        public Button left2;
        public Button left3;
        public Button left4;

        [Header("Trade Buttons")]public TextMeshProUGUI tradeLabel;
        public Button tradeBtn;
        public Button trade1;
        public Button trade2;
        public Button trade3;
        public Button trade4;
        
        [Header("Accept, Reject & Send")]
        public Button acceptBtn;
        public Button rejectBtn;
        public Button sendBtn;
        
        

        [Header("Other")]public TextMeshProUGUI showSelectedPlayer;
        public TextMeshProUGUI showSelectedMove;
        
        private Direction _currentlySelectedMove;
        private PlayerController _selectedPlayer;
        private PlayerTrade _selectedTrade;
        


        // Start is called before the first frame update
        void Start()
        {
            _playerController = gameHandler.GetPlayerController(player);
            _playerController.AddTradeObserver(this);
            UpdateMoveSprites();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {

                    PlayerController pc = hit.transform.GetComponent<PlayerController>();
                    if (pc != null)
                    {
                        _selectedPlayer = pc;
                        showSelectedPlayer.text = pc.player.ToString();

                    }
                }
            }
        }

        private void UpdateMoveSprites()
        {
            Direction[] moves = _playerController.GetMoves();


            left1.GetComponent<Image>().sprite = gameHandler.GetSprite(moves[0]);
            left2.GetComponent<Image>().sprite = gameHandler.GetSprite(moves[1]);
            left3.GetComponent<Image>().sprite = gameHandler.GetSprite(moves[2]);
            left4.GetComponent<Image>().sprite = gameHandler.GetSprite(moves[3]);
        }

        public Direction GetMoveFromBtn(Button b)
        {
            string spriteName = b.GetComponent<Image>().sprite.name;

            switch (spriteName)
            {
                case "Arrow_down":
                    return Direction.Down;
                case "Arrow_up":
                    return Direction.Up;
                case "Arrow_left":
                    return Direction.Left;
                case "Arrow_right":
                    return Direction.Right;
                default:
                    throw new Exception("Switch case Error");
            }
        }

        public void Left1BtnHit()
        {
            _currentlySelectedMove = GetMoveFromBtn(left1);
            showSelectedMove.text = _currentlySelectedMove.ToString();
        }
        public void Left2BtnHit()
        {
            _currentlySelectedMove = GetMoveFromBtn(left2);
            showSelectedMove.text = _currentlySelectedMove.ToString();
        }
        public void Left3BtnHit()
        {
            _currentlySelectedMove = GetMoveFromBtn(left3);
            showSelectedMove.text = _currentlySelectedMove.ToString();
        }
        public void Left4BtnHit()
        {
            _currentlySelectedMove = GetMoveFromBtn(left4);
            showSelectedMove.text = _currentlySelectedMove.ToString();
        }
        public void Trade1BtnHit()
        {
            if (trades[0] != null)
            {
                _selectedTrade = trades[0];
                tradeLabel.text = "Trade Offers: " + trades[0].Print();
            }
        }
        public void Trade2BtnHit()
        {
            if (trades[1] != null)
            {
                _selectedTrade = trades[1];
                tradeLabel.text = "Trade Offers: " + trades[1].Print();
            }
        }
        public void Trade3BtnHit()
        {
            if (trades[2] != null)
            {
                _selectedTrade = trades[2];
                tradeLabel.text = "Trade Offers: " + trades[2].Print();
            }
        }
        public void Trade4BtnHit()
        {
            if (trades[3] != null)
            {
                _selectedTrade = trades[3];
                tradeLabel.text = "Trade Offers: " + trades[3].Print();
            }
        }

        public void AcceptBtnHit()
        {
            if (_selectedTrade != null && _currentlySelectedMove != Direction.Blank)
            {
                _selectedTrade.AcceptTrade(_currentlySelectedMove,_playerController);
            }
            else
            {
                throw new Exception("Can't accept trade");
            }
        }
       

        public void TradeBtnHit()
        {
            if (_selectedPlayer == null || _currentlySelectedMove == null)
            {
                Debug.LogError("that didn't work",this);
            }
            
            gameHandler.OfferMove(_currentlySelectedMove,_selectedPlayer.player,_playerController.player);
        }

        public void NewTrade()
        {
            trades = _playerController.GetTrades();
            print(this.name + "Was notified");

            List<Button> buttons = new List<Button> {trade1, trade2, trade3, trade4};

            int i = 0;
            foreach (PlayerTrade trade in trades)
            {
                buttons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = trade.Print();
                i++;
            }
        }
    }
}
    
