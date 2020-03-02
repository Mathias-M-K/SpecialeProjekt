using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Container;
using UnityEngine;
using UnityEngine.AI;

namespace CoreGame
{
    public class PlayerController : MonoBehaviour
    {
        public Player player;
        [Space] public NavMeshAgent agent;

        private Camera _cam;
        private GameHandler _gameHandler;

        [SerializeField] private Direction[] moves = {Direction.Up, Direction.Down, Direction.Left, Direction.Right};
        public List<PlayerTrade> trades = new List<PlayerTrade>();
        private readonly List<ITradeObserver> _tradeObservers = new List<ITradeObserver>();
        private readonly List<IMoveObserver> _moveObservers = new List<IMoveObserver>();



        // Update basically contains the "click with mouse" functionality, and that only
        void Update()
        {
            /*if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

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
            }*/
        }

        //Telling game handler that the position is occupied
        private void AnnouncePosition(Vector3 position)
        {
            _gameHandler.RegisterPosition(player, position);
        }

        //Accept/Reject move from a given player
        public void AcceptTradeFrom(Player offeringPlayer, Direction counterOffer)
        {
            PlayerTrade tradeToBeAccepted = null;
            foreach (PlayerTrade playerTrade in trades)
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
            foreach (PlayerTrade playerTrade in trades)
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

        //Queuing trade for player to accept or reject
        public void QueTrade(PlayerTrade playerTrade)
        {
            trades.Add(playerTrade);
            
            //Code for notifying player that a trade is available @Lasse
        }

        //Add move
        public void AddMove(Direction d, int index)
        {
            moves[index] = d;
            NotifyMoveObservers();
        }

        //Removing move at index
        public void RemoveMove(int index)
        {
            moves[index] = Direction.Blank;
            NotifyMoveObservers();
        }

        //Returns the index of where the move/direction d is stored
        public int GetDirectionIndex(Direction d)
        {
            int keyIndex = Array.FindIndex(moves, w => w == d);
            return keyIndex;
        }

        public Direction[] GetMoves()
        {
            return moves;
        }

        public List<PlayerTrade> GetTrades()
        {
            return trades;
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
                default:
                    newGridPos = new Vector3(agentPos.x, agentPos.y, agentPos.z);
                    break;
            }

            //Creating navMeshPath and testing if is possible
            NavMeshPath navMeshPath = new NavMeshPath();
            agent.CalculatePath(newGridPos, navMeshPath);


            if (navMeshPath.status == NavMeshPathStatus.PathInvalid)
            {
                throw new ArgumentException("Position not reachable");
            }

            if (_gameHandler.IsPositionOccupied(newGridPos))
            {
                throw new Exception("Position occupied");
            }

            AnnouncePosition(newGridPos);
            agent.SetDestination(newGridPos);
        }

        //Player object will find it's way to the position
        public void MoveToPos(float x, float z)
        {
            agent.SetDestination(new Vector3(x, 1.5f, z));
        }

        //Calculate position on grid from mouse pointer
        private Vector3 CalculateGridPos(Vector3 point)
        {
            double x = Math.Floor(point.x) + 0.5f;
            double z = Math.Floor(point.z) + 0.5f;

            Vector3 gridPos = new Vector3((float) x, point.y, (float) z);

            return gridPos;
        }

        public void SetColor(Material material)
        {
            GetComponent<Renderer>().material = material;

            switch (material.name)
            {
                case "Blue":
                    player = Player.Blue;
                    break;
                case "Green":
                    player = Player.Green;
                    break;
                case "Red":
                    player = Player.Red;
                    break;
                case "Yellow":
                    player = Player.Yellow;
                    break;
                default:
                    throw new ArgumentException("Color not valid");
            }
        }

        public void SetCamera(Camera camera)
        {
            _cam = camera;
        }

        public void SetGameHandler(GameHandler gameHandler)
        {
            _gameHandler = gameHandler;
        }

        public void AddTradeObserver(ITradeObserver ito)
        {
            _tradeObservers.Add(ito);
        }

        public void AddMoveObserver(IMoveObserver imo)
        {
            _moveObservers.Add(imo);
        }

        public void NotifyTradeObservers()
        {
            foreach (ITradeObserver observer in _tradeObservers)
            {
                observer.TradeUpdate();
            }
        }

        public void NotifyMoveObservers()
        {
            foreach (IMoveObserver observer in _moveObservers)
            {
                observer.MoveInventoryUpdate();
            }
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
        Yellow
    }
}