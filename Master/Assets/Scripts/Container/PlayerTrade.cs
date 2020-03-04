using System;
using System.CodeDom;
using CoreGame;
using UnityEngine;

namespace Container
{
    public class PlayerTrade
    {
        public readonly Player OfferingPlayer; //Player offering a trade
        private readonly Player _receivingPlayer; //Player to which the move is being offered
        private readonly Direction _direction; //the move being offered
        private readonly int _storedMoveIndex; //The index at which the offered move is stored

        private readonly GameHandler _gameHandler;

        public PlayerTrade(Player offeringPlayer, Player receivingPlayer, Direction direction, GameHandler gameHandler,
            int storedMoveIndex)
        {
            OfferingPlayer = offeringPlayer;
            _direction = direction;
            _gameHandler = gameHandler;
            _storedMoveIndex = storedMoveIndex;
            _receivingPlayer = receivingPlayer;
        }

        public void AcceptTrade(Direction counteroffer, PlayerController acceptingPlayer)
        {
            if (acceptingPlayer.player != _receivingPlayer)
                throw new Exception($"Offer was not for {acceptingPlayer.player}");
            
            if (acceptingPlayer.GetIndexForDirection(counteroffer) == -1)
                throw new ArgumentException($"{acceptingPlayer.player} does not posses the move {counteroffer}");

            if (counteroffer == Direction.Blank) 
                throw new ArgumentException("Can't use blank to trade with");

            PlayerController offeringPlayerController = _gameHandler.GetPlayerController(OfferingPlayer);
            offeringPlayerController.AddMove(counteroffer, _storedMoveIndex);
            acceptingPlayer.AddMove(_direction, acceptingPlayer.GetIndexForDirection(counteroffer));

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

            _gameHandler.GetPlayerController(OfferingPlayer).AddMove(_direction, _storedMoveIndex);

            _gameHandler.trades.Remove(this);
            rejectingPlayer.trades.Remove(this);
        }

        public string Print()
        {
            return OfferingPlayer + " offering: " + _direction;
        }
    }
}