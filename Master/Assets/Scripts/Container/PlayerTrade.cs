using System;
using System.CodeDom;
using CoreGame;
using UnityEngine;

namespace Container
{
    public class PlayerTrade
    {
        public readonly Player OfferingOfferingPlayer;
        private readonly Player _receivingPlayer;
        private readonly Direction _direction;
        private readonly int _index;

        private readonly GameHandler _gameHandler;

        public PlayerTrade(Player offeringPlayer, Player receivingPlayer, Direction direction, GameHandler gameHandler, int index)
        {
            OfferingOfferingPlayer = offeringPlayer;
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

            _gameHandler.GetPlayerController(OfferingOfferingPlayer).AddMove(counteroffer, _index);
            acceptingPlayer.AddMove(_direction, acceptingPlayer.GetDirectionIndex(counteroffer));

            _gameHandler.trades.Remove(this);
            acceptingPlayer.trades.Remove(this);
        }

        public void RejectTrade(PlayerController rejectingPlayer)
        {
            if (rejectingPlayer.player != _receivingPlayer)
            {
                throw new Exception($"Offer was not for {rejectingPlayer.player}");
            }

            _gameHandler.GetPlayerController(OfferingOfferingPlayer).AddMove(_direction, _index);

            _gameHandler.trades.Remove(this);
            rejectingPlayer.trades.Remove(this);
        }

        public string Print()
        {
            return OfferingOfferingPlayer + " offering: " + _direction;
        }
    }
}