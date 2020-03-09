using System;
using System.CodeDom;
using CoreGame;
using UnityEngine;

namespace Container
{
    public class PlayerTrade
    {
        public readonly Player OfferingPlayer; //Player offering a trade
        public readonly Player ReceivingPlayer; //Player to which the move is being offered
        public readonly Direction Direction; //the move being offered
        private readonly int _storedMoveIndex; //The index at which the offered move is stored

        private readonly GameHandler _gameHandler;

        public PlayerTrade(Player offeringPlayer, Player receivingPlayer, Direction direction, GameHandler gameHandler,
            int storedMoveIndex)
        {
            OfferingPlayer = offeringPlayer;
            Direction = direction;
            _gameHandler = gameHandler;
            _storedMoveIndex = storedMoveIndex;
            ReceivingPlayer = receivingPlayer;
        }

        public void AcceptTrade(Direction counteroffer, PlayerController acceptingPlayer)
        {
            if (acceptingPlayer.player != ReceivingPlayer)
                throw new Exception($"Offer was not for {acceptingPlayer.player}");
            
            if (acceptingPlayer.GetIndexForDirection(counteroffer) == -1)
                throw new ArgumentException($"{acceptingPlayer.player} does not posses the move {counteroffer}");

            if (counteroffer == Direction.Blank) 
                throw new ArgumentException("Can't use blank to trade with");

            PlayerController offeringPlayerController = _gameHandler.GetPlayerController(OfferingPlayer);
            
            offeringPlayerController.AddMove(counteroffer, _storedMoveIndex);
            acceptingPlayer.AddMove(Direction, acceptingPlayer.GetIndexForDirection(counteroffer));

            offeringPlayerController.RemoveOutgoingTrade(this);
            acceptingPlayer.RemoveIncomingTrade(this);
            

            _gameHandler.trades.Remove(this);
            
        }

        public void RejectTrade(PlayerController rejectingPlayer)
        {
            if (rejectingPlayer.player != ReceivingPlayer)
            {
                throw new Exception($"Offer was not for {rejectingPlayer.player}");
            }

            PlayerController offeringPlayerController = _gameHandler.GetPlayerController(OfferingPlayer);
            
            offeringPlayerController.AddMove(Direction, _storedMoveIndex);

            offeringPlayerController.RemoveOutgoingTrade(this);
            rejectingPlayer.RemoveIncomingTrade(this);
            
            offeringPlayerController.NotifyTradeObservers();
            rejectingPlayer.NotifyTradeObservers();
            
            _gameHandler.trades.Remove(this);
        }

        public void CancelTrade(Player cancellingPlayer)
        {
            if (cancellingPlayer != OfferingPlayer) throw new ArgumentException("Only the player that created the trade can cancel it");
            
            RejectTrade(_gameHandler.GetPlayerController(ReceivingPlayer));
        }
        

        public string Print()
        {
            return OfferingPlayer + " offering: " + Direction;
        }
    }
}