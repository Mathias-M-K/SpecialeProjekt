using System;
using System.CodeDom;
using CoreGame;
using UnityEngine;

namespace Container
{
    public class PlayerTrade
    {
        public readonly Player OfferingPlayer;
        private readonly Player _receivingPlayer;
        private readonly Direction _direction;
        private readonly int _index;

        private readonly GameHandler _gameHandler;

        public PlayerTrade(Player offeringPlayer, Player receivingPlayer, Direction direction, GameHandler gameHandler, int index)
        {
            OfferingPlayer = offeringPlayer;
            _direction = direction;
            _gameHandler = gameHandler;
            _index = index;
            _receivingPlayer = receivingPlayer;
        }

        public void AcceptTrade(Direction counteroffer, PlayerController acceptingPlayer)
        {
            if (acceptingPlayer.player != _receivingPlayer)
            {
                throw new Exception($"Offer was not for {acceptingPlayer.player}");
            }

            if (acceptingPlayer.GetDirectionIndex(counteroffer) == -1)
            {
                throw new ArgumentException($"{acceptingPlayer.player} does not posses the move {counteroffer}");
            }

            PlayerController offeringPlayerController = _gameHandler.GetPlayerController(OfferingPlayer); 
            offeringPlayerController.AddMove(counteroffer, _index);
            acceptingPlayer.AddMove(_direction, acceptingPlayer.GetDirectionIndex(counteroffer));
            
            offeringPlayerController.NotifyTradeObservers();
            acceptingPlayer.NotifyTradeObservers();

            _gameHandler.trades.Remove(this);
            acceptingPlayer.trades.Remove(this);
            
            
        }

        public void RejectTrade(PlayerController rejectingPlayer)
        {
            if (rejectingPlayer.player != _receivingPlayer)
            {
                throw new Exception($"Offer was not for {rejectingPlayer.player}");
            }

            _gameHandler.GetPlayerController(OfferingPlayer).AddMove(_direction, _index);

            _gameHandler.trades.Remove(this);
            rejectingPlayer.trades.Remove(this);
        }

        public string Print()
        {
            return OfferingPlayer + " offering: " + _direction;
        }
    }
}