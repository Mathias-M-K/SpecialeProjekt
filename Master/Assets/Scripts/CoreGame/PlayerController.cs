﻿using System;
using System.Collections.Generic;
using Container;
using CoreGame.Interfaces;
using CoreGame.Strategies.Implementations.PlayerFinishImplementations;
using CoreGame.Strategies.Interfaces;
using CoreGame.Strategies.PlayerFinishImplementations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

namespace CoreGame
{
    public class PlayerController : MonoBehaviour
    {
        public Player player;
        [Space] public NavMeshAgent agent;
        [Header("Settings")] public bool enableMouseMovement;
        [Header("Strategies")] public PlayerFinishStrategyEnum playerFinishStrategy;

        //Strategies
        private _PlayerFinishStrategy _playerFinishStrategy;

        //List of moves available for the player
        private Direction[] _moves = {Direction.Up, Direction.Down, Direction.Left, Direction.Right};

        //List of trades
        public List<PlayerTrade> incomingTradeOffers = new List<PlayerTrade>();
        public List<PlayerTrade> outgoingTradeOffers = new List<PlayerTrade>();

        //Observers
        private readonly List<ITradeObserver> _tradeObservers = new List<ITradeObserver>();
        private readonly List<IMoveObserver> _moveObservers = new List<IMoveObserver>();
        private List<IReadyObserver> _readyObservers = new List<IReadyObserver>();

        //Other variables
        private bool ready;
        public bool Ready
        {
            get => ready;
            set
            {
                ready = value; 
                NotifyReadyObservers();
            }
        }

        private void Start()
        {
            InitializeStrategies();
        }

        // Update basically contains the "click with mouse" functionality, and that only
        private void Update()
        {
            if (!enableMouseMovement) return;

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.GetComponent<PlayerController>() != null)
                    {
                        return;
                    }

                    //If wall is hit, y will be 2.5
                    if (hit.point.y == 2.5f)
                    {
                        throw new Exception("That's is a wall");
                    }

                    agent.SetDestination(CalculateGridPos(hit.point));
                }
            }
        }

        //Telling game handler that the position is occupied
        private void AnnouncePosition(Vector3 position)
        {
            GameHandler.current.RegisterPosition(player, position);
        }

        public Vector2 GetPosition()
        {
            Vector3 pos = gameObject.transform.position;
            return new Vector2(pos.x,pos.z);
        }

        //Accept/Reject move from a given player
        public void AcceptTradeFrom(Player offeringPlayer, Direction counterOffer)
        {
            PlayerTrade tradeToBeAccepted = null;
            foreach (PlayerTrade playerTrade in incomingTradeOffers)
            {
                if (playerTrade.OfferingPlayer == offeringPlayer)
                {
                    tradeToBeAccepted = playerTrade;
                }
            }

            if (tradeToBeAccepted != null)
            {
                tradeToBeAccepted.AcceptTrade(counterOffer, this);
            }
            else
            {
                throw new Exception($"No trade offers from {offeringPlayer}");
            }
        }

        public void RejectTradeFrom(Player offeringPlayer)
        {
            PlayerTrade tradeToBeAccepted = null;
            foreach (PlayerTrade playerTrade in incomingTradeOffers)
            {
                if (playerTrade.OfferingPlayer == offeringPlayer)
                {
                    tradeToBeAccepted = playerTrade;
                }
            }

            if (tradeToBeAccepted != null)
            {
                tradeToBeAccepted.RejectTrade(this);
            }
            else
            {
                throw new Exception($"No trade offers from {offeringPlayer}");
            }
        }

        //Create a trade and send it to game handler
        public void CreateTrade(Direction direction, Player receivingPlayer)
        {
            //Doing some checks
            //Checking that the move is in inventory
            if (GetIndexForDirection(direction) == -1)
                throw new ArgumentException($"the move {direction} is not in inventory");

            if (direction == Direction.Blank) throw new ArgumentException("You can't trade a blank move");

            //Checking that the player is not trying to trade to himself
            if (receivingPlayer == player) throw new ArgumentException($"{player} is trying to trade to himself");

            GameHandler.current.NewTrade(direction, GetIndexForDirection(direction), receivingPlayer, player);
            RemoveMove(GetIndexForDirection(direction));
        }

        //Queuing trade for player to accept or reject
        public void AddIncomingTrade(PlayerTrade playerTrade)
        {
            incomingTradeOffers.Add(playerTrade);
        }

        public void AddOutgoingTrade(PlayerTrade playerTrade)
        {
            outgoingTradeOffers.Add(playerTrade);
        }

        public void RemoveOutgoingTrade(PlayerTrade playerTrade)
        {
            outgoingTradeOffers.Remove(playerTrade);
        }

        public void RemoveIncomingTrade(PlayerTrade playerTrade)
        {
            incomingTradeOffers.Remove(playerTrade);
        }

        //Add and remove moves from the inventory
        public void AddMove(Direction d, int index)
        {
            _moves[index] = d;
            NotifyMoveObservers();
        }

        public void RemoveMove(int index)
        {
            _moves[index] = Direction.Blank;
            NotifyMoveObservers();
        }

        //Returns the index of where the move/direction d is stored
        public int GetIndexForDirection(Direction d)
        {
            int keyIndex = Array.FindIndex(_moves, w => w == d);
            return keyIndex;
        }

        public Direction[] GetMoves()
        {
            return _moves;
        }

        public List<PlayerTrade> GetIncomingTrades()
        {
            return incomingTradeOffers;
        }

        public List<PlayerTrade> GetOutgoingTrades()
        {
            return outgoingTradeOffers;
        }

        public List<ITradeObserver> GetTradeObservers()
        {
            return _tradeObservers;
        }

        //Move player one unit in a given direction
        public void MovePlayer(Direction d)
        {
            //The new position
            Vector3 newGridPos;

            Vector3 agentPos = agent.destination;
            switch (d)
            {
                case Direction.Up:
                    newGridPos = new Vector3(agentPos.x, agentPos.y, agentPos.z + 1);
                    break;
                case Direction.Down:
                    newGridPos = new Vector3(agentPos.x, agentPos.y, agentPos.z - 1);
                    break;
                case Direction.Left:
                    newGridPos = new Vector3(agentPos.x - 1, agentPos.y, agentPos.z);
                    break;
                case Direction.Right:
                    newGridPos = new Vector3(agentPos.x + 1, agentPos.y, agentPos.z);
                    break;
                case Direction.Blank:
                    throw new ArgumentException("MovePlayer can't be invoked with a blank move");
                default:
                    newGridPos = new Vector3(agentPos.x, agentPos.y, agentPos.z);
                    break;
            }

            //Creating navMeshPath and testing if is possible
            NavMeshPath navMeshPath = new NavMeshPath();
            agent.CalculatePath(newGridPos, navMeshPath);
            
            if (navMeshPath.status == NavMeshPathStatus.PathInvalid)
            {
                throw new InvalidOperationException("Path Invalid");
            }

            if (!GameHandler.current.playersCanPhase && GameHandler.current.IsPositionOccupied(new Vector2(newGridPos.x,newGridPos.z)))
            {
                throw new InvalidOperationException($"{player} is trying to phase though another player, while phaseAllowed is {GameHandler.current.playersCanPhase}");
            }

            AnnouncePosition(newGridPos);

            agent.SetDestination(newGridPos);
        }

        //Player object will find it's way to the position
        public void MoveToPos(Vector2 pos)
        {
            Vector3 newPos = CalculateGridPos(new Vector3(pos.x, 1, pos.y));
            AnnouncePosition(newPos);
            agent.SetDestination(newPos);
        }
        public void MoveToPos(float x, float y)
        {
            Vector2 pos = new Vector2(x,y);
            MoveToPos(pos);
        }

        //Calculate position on grid from mouse pointer
        private Vector3 CalculateGridPos(Vector3 point)
        { 
            double x = Math.Round(point.x);
            double z = Math.Round(point.z);

            Vector3 gridPos = new Vector3((float) x, point.y, (float) z);

            return gridPos;
        }

        //Resets the moves for the player
        public void ResetAfterSequence()
        {
            Direction[] defaultMoves = {Direction.Up, Direction.Down, Direction.Left, Direction.Right};
            _moves = defaultMoves;

            Ready = false;

            NotifyMoveObservers();
        }

        public void SetPlayer(Player newPlayerTag)
        {
            player = newPlayerTag;
            GetComponent<Renderer>().material.color = ColorPalette.current.GetPlayerColor(newPlayerTag);
        }

        
        /*
         * Trade Observer Methods
         */
        public void AddTradeObserver(ITradeObserver ito)
        {
            _tradeObservers.Add(ito);
        }
        public void RemoveTradeObserver(ITradeObserver ito)
        {
            _tradeObservers.Remove(ito);
        }
        public void NotifyTradeObservers(PlayerTrade trade, TradeActions tradeAction)
        {
            foreach (ITradeObserver observer in _tradeObservers)
            {
                observer.OnNewTradeActivity(trade, tradeAction);
            }
        }
        
        /*
         * Move Observer Methods
         */
        public void AddMoveObserver(IMoveObserver imo)
        {
            _moveObservers.Add(imo);
        }
        public void RemoveMoveObserver(IMoveObserver imo)
        {
            _moveObservers.Remove(imo);
        }
        public void NotifyMoveObservers()
        {
            foreach (IMoveObserver observer in _moveObservers)
            {
                observer.OnMoveInventoryChange(GetMoves());
            }
        }
        
        /*
         * Ready Observer Methods
         */
        public void AddReadyObserver(IReadyObserver iro)
        {
            _readyObservers.Add(iro);
        }

        public void RemoveReadyObserver(IReadyObserver iro)
        {
            _readyObservers.Remove(iro);
        }

        public void NotifyReadyObservers()
        {
            foreach (IReadyObserver observer in _readyObservers)
            {
                observer.OnReadyStateChanged(ready);
            }
        }

        public void Die()
        {
            _playerFinishStrategy.PlayerFinish(this);
        }

        private void InitializeStrategies()
        {
            switch (playerFinishStrategy)
            {
                case PlayerFinishStrategyEnum.Remove:
                    _playerFinishStrategy = new RemovePlayerFinishStrategy();
                    break;
                case PlayerFinishStrategyEnum.AbleToTrade:
                    _playerFinishStrategy = new AllowTradePlayerFinishStrategy();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }

    public enum Direction
    {
        Up,
        Down,
        Right,
        Left,
        Blank
    }

    public enum Player
    {
        Red,
        Blue,
        Green,
        Yellow,
    }

    public enum PlayerFinishStrategyEnum
    {
        Remove,
        AbleToTrade
    }
}