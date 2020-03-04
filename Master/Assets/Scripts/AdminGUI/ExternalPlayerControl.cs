
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
    public class ExternalPlayerControl : MonoBehaviour,ITradeObserver,IMoveObserver
    {
        [Header("Set Player")] public Player player;
        [Header("Set Game Handler")] public GameHandler gameHandler;
        private PlayerController playerController;
        private List<PlayerTrade> _trades;
        
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
        public TextMeshProUGUI playerLabel;
        
        private Direction _currentlySelectedMove = Direction.Blank;
        private PlayerController _selectedPlayer;
        private PlayerTrade _selectedTrade;
        


        // Start is called before the first frame update
        void Start()
        {
            playerLabel.text = player.ToString();
            
            playerController = gameHandler.GetPlayerController(player);

            if (playerController == null) return;
            playerController.AddTradeObserver(this);
            playerController.AddMoveObserver(this);
            UpdateMoveSprites();
        }
        
        private void UpdateMoveSprites()
        {
            Direction[] moves = playerController.GetMoves();


            left1.GetComponent<Image>().sprite = gameHandler.GetSprite(moves[0]);
            left2.GetComponent<Image>().sprite = gameHandler.GetSprite(moves[1]);
            left3.GetComponent<Image>().sprite = gameHandler.GetSprite(moves[2]);
            left4.GetComponent<Image>().sprite = gameHandler.GetSprite(moves[3]);
        }

        private void UpdateTrades()
        {
            _trades = playerController.GetTrades();

            List<Button> buttons = new List<Button> {trade1, trade2, trade3, trade4};

            foreach (Button button in buttons)
            {
                button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
            }
            
            int i = 0;
            foreach (PlayerTrade trade in _trades)
            {
                buttons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = trade.Print();
                i++;
            }
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
            if (_trades[0] != null)
            {
                _selectedTrade = _trades[0];
                tradeLabel.text = "Trade Offers: " + _trades[0].Print();
            }
        }
        public void Trade2BtnHit()
        {
            if (_trades[1] != null)
            {
                _selectedTrade = _trades[1];
                tradeLabel.text = "Trade Offers: " + _trades[1].Print();
            }
        }
        public void Trade3BtnHit()
        {
            if (_trades[2] != null)
            {
                _selectedTrade = _trades[2];
                tradeLabel.text = "Trade Offers: " + _trades[2].Print();
            }
        }
        public void Trade4BtnHit()
        {
            if (_trades[3] != null)
            {
                _selectedTrade = _trades[3];
                tradeLabel.text = "Trade Offers: " + _trades[3].Print();
            }
        }

        public void RedBtnHit()
        {
            if (player != Player.Red)
            {
                _selectedPlayer = gameHandler.GetPlayerController(Player.Red);
                showSelectedPlayer.text = Player.Red.ToString();
            }
        }
        public void BlueBtnHit()
        {
            if (player != Player.Blue)
            {
                _selectedPlayer = gameHandler.GetPlayerController(Player.Blue);
                showSelectedPlayer.text = Player.Blue.ToString();
            }
        }
        public void YellowBtnHit()
        {
            if (player != Player.Yellow)
            {
                _selectedPlayer = gameHandler.GetPlayerController(Player.Yellow);
                showSelectedPlayer.text = Player.Yellow.ToString();
            }
        }
        public void GreenBtnHit()
        {
            if (player != Player.Green)
            {
                _selectedPlayer = gameHandler.GetPlayerController(Player.Green);
                showSelectedPlayer.text = Player.Green.ToString();
            }
        }

        public void AcceptBtnHit()
        {
            if (_selectedTrade != null && _currentlySelectedMove != Direction.Blank)
            {
                _selectedTrade.AcceptTrade(_currentlySelectedMove,playerController);
                UpdateTrades();
                UpdateMoveSprites();
            }
            else
            {
                throw new Exception("Can't accept trade");
            }
        }
        public void RejectBtnHit()
        {
            if (_selectedTrade != null)
            {
                _selectedTrade.RejectTrade(playerController);
                gameHandler.GetPlayerController(_selectedTrade.OfferingPlayer).NotifyMoveObservers();
                UpdateTrades();
                UpdateMoveSprites();
            }
            else
            {
                throw new Exception("Can't accept trade");
            }
        }
        public void TradeBtnHit()
        {
            if (_selectedPlayer == null || _currentlySelectedMove == Direction.Blank)
            {
                Debug.LogError("that didn't work",this);
            }
            
            playerController.CreateTrade(_currentlySelectedMove,_selectedPlayer.player);
        }

        public void SendBtnHit()
        {
            if (_currentlySelectedMove == Direction.Blank) return;
            
            gameHandler.AddMoveToSequence(player,_currentlySelectedMove);
            
        }

        
        
        public void TradeUpdate()
        {
            print(this.name + "Was notified");
            UpdateTrades();
        }

        public void MoveInventoryUpdate(Direction[] directions)
        {
            UpdateMoveSprites();
        }
    }
}
    
