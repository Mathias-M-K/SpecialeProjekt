using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        private List<ITradeObserver> _statObservers;

        private readonly GameHandler _gameHandler;

        public PlayerTrade(Player offeringPlayer, Player receivingPlayer, Direction directionOffer, GameHandler gameHandler, int storedMoveIndex,List<ITradeObserver> observers)
        {
            TradeID = Random.Range(0, 10000);
            OfferingPlayer = offeringPlayer;
            DirectionOffer = directionOffer;
            _gameHandler = gameHandler;
            _storedMoveIndex = storedMoveIndex;
            ReceivingPlayer = receivingPlayer;
            _statObservers = observers;
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
            
            NotifyObservers(TradeActions.TradeAccepted);
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
            
            //offeringPlayerController.NotifyTradeObservers(this,TradeActions.TradeRejected);
            //rejectingPlayer.NotifyTradeObservers(this,TradeActions.TradeRejected);
            
            NotifyObservers(TradeActions.TradeRejected);
            _gameHandler.trades.Remove(this);
        }

        public void CancelTrade(Player cancellingPlayer)
        {
            if (cancellingPlayer != OfferingPlayer) throw new ArgumentException("Only the player that created the trade can cancel it");
            
            NotifyObservers(TradeActions.TradeCanceled);
            
            PlayerController offeringPlayer = _gameHandler.GetPlayerController(OfferingPlayer);
            PlayerController rejectingPlayer = GameHandler.current.GetPlayerController(ReceivingPlayer);
            
            offeringPlayer.AddMove(DirectionOffer, _storedMoveIndex);

            offeringPlayer.RemoveOutgoingTrade(this);
            rejectingPlayer.RemoveIncomingTrade(this);
            
        }
        

        public string Print()
        {
            return OfferingPlayer + " offering: " + DirectionOffer;
        }

        public void NotifyObservers(TradeActions tradAction)
        {
            foreach (ITradeObserver observer in _statObservers)
            {
                observer.OnNewTradeActivity(this,tradAction);
            }
        }
    }
}