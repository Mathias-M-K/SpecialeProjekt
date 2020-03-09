using System;
using System.Collections.Generic;
using CoreGame;
using CoreGame.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Container
{
    public class PlayerTrade
    {
        public readonly Player OfferingPlayer; //Player offering a trade
        public readonly Player ReceivingPlayer; //Player to which the move is being offered
        public readonly Direction DirectionOffer; //the move being offered
        public Direction DirectionCounterOffer = Direction.Blank;

        public int TradeID;
        private readonly int _storedMoveIndex; //The index at which the offered move is stored
        private readonly List<IStatObserver> _statObservers;

        private readonly GameHandler _gameHandler;

        public PlayerTrade(Player offeringPlayer, Player receivingPlayer, Direction directionOffer, GameHandler gameHandler, int storedMoveIndex,List<IStatObserver> observers)
        {
            TradeID = Random.Range(0, 10000);
            OfferingPlayer = offeringPlayer;
            DirectionOffer = directionOffer;
            _gameHandler = gameHandler;
            _storedMoveIndex = storedMoveIndex;
            ReceivingPlayer = receivingPlayer;
            _statObservers = observers;

            foreach (IStatObserver observer in _statObservers)
            {
                observer.NewTradeActivity(this,"Pending");
            }
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

            DirectionCounterOffer = counteroffer;
            
            offeringPlayerController.AddMove(counteroffer, _storedMoveIndex);
            acceptingPlayer.AddMove(DirectionOffer, acceptingPlayer.GetIndexForDirection(counteroffer));

            offeringPlayerController.RemoveOutgoingTrade(this);
            acceptingPlayer.RemoveIncomingTrade(this);
            
            NotifyObservers("Accepted");
            _gameHandler.trades.Remove(this);
        }

        public void RejectTrade(PlayerController rejectingPlayer)
        {
            if (rejectingPlayer.player != ReceivingPlayer)
            {
                throw new Exception($"Offer was not for {rejectingPlayer.player}");
            }

            PlayerController offeringPlayerController = _gameHandler.GetPlayerController(OfferingPlayer);
            
            offeringPlayerController.AddMove(DirectionOffer, _storedMoveIndex);

            offeringPlayerController.RemoveOutgoingTrade(this);
            rejectingPlayer.RemoveIncomingTrade(this);
            
            offeringPlayerController.NotifyTradeObservers();
            rejectingPlayer.NotifyTradeObservers();
            
            NotifyObservers("Rejected");
            _gameHandler.trades.Remove(this);
        }

        public void CancelTrade(Player cancellingPlayer)
        {
            if (cancellingPlayer != OfferingPlayer) throw new ArgumentException("Only the player that created the trade can cancel it");
            
            NotifyObservers("Canceled");
            RejectTrade(_gameHandler.GetPlayerController(ReceivingPlayer));
        }
        

        public string Print()
        {
            return OfferingPlayer + " offering: " + DirectionOffer;
        }

        private void NotifyObservers(string status)
        {
            foreach (IStatObserver observer in _statObservers)
            {
                observer.NewTradeActivity(this,status);
            }
        }
    }
}