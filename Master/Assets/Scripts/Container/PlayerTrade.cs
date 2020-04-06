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
        public readonly PlayerTags OfferingPlayerTags; //Player offering a trade
        public readonly PlayerTags ReceivingPlayerTags; //Player to which the move is being offered
        public readonly Direction DirectionOffer; //the move being offered
        public Direction DirectionCounterOffer = Direction.Blank;

        public int TradeID;

        
        private readonly int _storedMoveIndex; //The index at which the offered move is stored
        private List<ITradeObserver> _statObservers;

        private readonly GameHandler _gameHandler;

        public PlayerTrade(PlayerTags offeringPlayerTags, PlayerTags receivingPlayerTags, Direction directionOffer, GameHandler gameHandler, int storedMoveIndex,List<ITradeObserver> observers)
        {
            TradeID = Random.Range(0, 10000);
            OfferingPlayerTags = offeringPlayerTags;
            DirectionOffer = directionOffer;
            _gameHandler = gameHandler;
            _storedMoveIndex = storedMoveIndex;
            ReceivingPlayerTags = receivingPlayerTags;
            _statObservers = observers;
        }

        public void AcceptTrade(Direction counteroffer, PlayerController acceptingPlayer)
        {
            if (acceptingPlayer.playerTags != ReceivingPlayerTags)
                throw new Exception($"Offer was not for {acceptingPlayer.playerTags}");
            
            if (acceptingPlayer.GetIndexForDirection(counteroffer) == -1)
                throw new ArgumentException($"{acceptingPlayer.playerTags} does not posses the move {counteroffer}");

            if (counteroffer == Direction.Blank) 
                throw new ArgumentException("Can't use blank to trade with");

            PlayerController offeringPlayerController = _gameHandler.GetPlayerController(OfferingPlayerTags);

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
            if (rejectingPlayer.playerTags != ReceivingPlayerTags)
            {
                throw new Exception($"Offer was not for {rejectingPlayer.playerTags}");
            }

            PlayerController offeringPlayerController = _gameHandler.GetPlayerController(OfferingPlayerTags);
            
            offeringPlayerController.AddMove(DirectionOffer, _storedMoveIndex);

            offeringPlayerController.RemoveOutgoingTrade(this);
            rejectingPlayer.RemoveIncomingTrade(this);
            
            //offeringPlayerController.NotifyTradeObservers(this,TradeActions.TradeRejected);
            //rejectingPlayer.NotifyTradeObservers(this,TradeActions.TradeRejected);
            
            NotifyObservers(TradeActions.TradeRejected);
            _gameHandler.trades.Remove(this);
        }

        private void CancelTrade()
        {
            PlayerController offeringPlayer = _gameHandler.GetPlayerController(OfferingPlayerTags);
            PlayerController rejectingPlayer = GameHandler.Current.GetPlayerController(ReceivingPlayerTags);
            
            offeringPlayer.AddMove(DirectionOffer, _storedMoveIndex);

            offeringPlayer.RemoveOutgoingTrade(this);
            rejectingPlayer.RemoveIncomingTrade(this);
        }

        public void CancelTrade(PlayerTags cancellingPlayerTags)
        {
            if (cancellingPlayerTags != OfferingPlayerTags) throw new ArgumentException("Only the player that created the trade can cancel it");
            
            NotifyObservers(TradeActions.TradeCanceled);
            CancelTrade();
        }
        public void CancelTrade(GameHandler gameHandler)
        {
            NotifyObservers(TradeActions.TradeCanceledByGameHandler);
            CancelTrade();
        }
        
        public string Print()
        {
            return OfferingPlayerTags + " offering: " + DirectionOffer;
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