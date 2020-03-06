using System;
using System.Collections;
using System.Collections.Generic;
using Container;
using CoreGame.Interfaces;
using UnityEngine;

namespace CoreGame
{
    public class GameHandler : MonoBehaviour,IFinishPointObserver
    {
        public List<PlayerTrade> trades = new List<PlayerTrade>();
        
        private readonly List<PlayerMove> _sequenceMoves = new List<PlayerMove>();
        private readonly List<PlayerController> _players = new List<PlayerController>();
        private readonly List<Vector3> _spawnPositions = new List<Vector3>();
        private readonly List<ISequenceObserver> _sequenceObservers = new List<ISequenceObserver>();
        private readonly Vector3[] _occupiedPositions = new Vector3[4];
        
        [Header("Player Prefab")] public GameObject player;
        [Header("Goal")] public FinishPointController finishPointObject;
        
        [Space] [Header("Settings")] [Range(1, 4)]
        public int numberOfPlayers;

        [Space] [Header("Player Abilities")]
        public bool playersCanPhase;
        
        [Space] [Header("Materials")] public Material redMaterial;
        public Material blueMaterial;
        public Material greenMaterial;
        public Material yellowMaterial;
        
        [Space] [Header("Sprites")] public Sprite leftSprite;
        public Sprite rightSprite;
        public Sprite upSprite;
        public Sprite downSprite;
        public Sprite blankSprite;

        public struct PlayerMove
        {
            public readonly Direction Direction;
            public readonly Player Player;

            public PlayerMove(Player p, Direction d)
            {
                Player = p;
                Direction = d;
            }
        }

        private void Awake()
        {
            _spawnPositions.Add(new Vector3(1.5f, 2, 10.5f));
            _spawnPositions.Add(new Vector3(1.5f, 2, 1.5f));
            _spawnPositions.Add(new Vector3(10.5f, 2, 1.5f));
            _spawnPositions.Add(new Vector3(10.5f, 2, 10.5f));

            finishPointObject.AddObserver(this);
            SpawnPlayers();
        }

        private void Start()
        {
            RemoveBarricadesForInactivePlayers();
        }

        public bool IsPositionOccupied(Vector3 position)
        {
            foreach (Vector3 occupiedPosition in _occupiedPositions)
            {
                if (position.x == occupiedPosition.x && position.z == occupiedPosition.z)
                {
                    return true;
                }
            }

            return false;
        }

        //Lets players announce their position
        public void RegisterPosition(Player player, Vector3 position)
        {
            switch (player)
            {
                case Player.Red:
                    _occupiedPositions[0] = position;
                    break;
                case Player.Blue:
                    _occupiedPositions[1] = position;
                    break;
                case Player.Green:
                    _occupiedPositions[2] = position;
                    break;
                case Player.Yellow:
                    _occupiedPositions[3] = position;
                    break;
                default:
                    throw new ArgumentException("Not a valid player");
            }
        }
        
        public void NewTrade(Direction direction, int directionIndex, Player playerReceiving, Player playerOffering)
        {
            PlayerController playerReceivingController = GetPlayerController(playerReceiving);
            PlayerController playerOfferingController = GetPlayerController(playerOffering);
            
            PlayerTrade trade = new PlayerTrade(playerOffering, playerReceiving, direction, this, directionIndex);

            trades.Add(trade);
            playerReceivingController.AddIncomingTrade(trade);
            playerOfferingController.AddOutgoingTrade(trade);
        }
        
        public void AddMoveToSequence(Player p, Direction d)
        {
            PlayerController playerController = GetPlayerController(p);
            
            if (playerController == null)
            {
                Debug.LogException(new ArgumentException(p + " is not active"), this);
                return;
            }

            if (playerController.GetIndexForDirection(d) == -1)
            {
                Debug.LogError($"{player} does not posses the {d} move");
                return;
            }

            PlayerMove playerMove = new PlayerMove(p, d);
            _sequenceMoves.Add(playerMove);
            
            playerController.RemoveMove(playerController.GetIndexForDirection(d));
            
            playerController.NotifyMoveObservers();
            NotifySequenceObservers();
        }

        public void RemoveMoveFromSequence(PlayerMove move)
        {
            _sequenceMoves.Remove(move);
            NotifySequenceObservers();
        }

        public IEnumerator PerformSequence(float delayBetweenMoves)
        {
            foreach (PlayerMove pm in _sequenceMoves)
            {
                PlayerController playerController = GetPlayerController(pm.Player);

                playerController.MovePlayer(pm.Direction);
                yield return new WaitForSeconds(delayBetweenMoves);
            }

            foreach (PlayerController playerController in _players)
            {
                playerController.ResetMoves();
            }
            
            _sequenceMoves.Clear();
            NotifySequenceObservers();
        }

        public PlayerController GetPlayerController(Player p)
        {
            foreach (var playerController in _players)
            {
                if (playerController.player == p)
                {
                    return playerController;
                }
            }

            return null;
        }

        public List<Vector3> GetSpawnLocations()
        {
            return _spawnPositions;
        }

        public List<PlayerMove> GetSequence()
        {
            return _sequenceMoves;
        }

        private void SpawnPlayers()
        {
            if (numberOfPlayers > 4)
            {
                Debug.LogError("Number of max players have been exceeded, fallback to 4 players", this);
                numberOfPlayers = 4;
            }

            List<Player> playerColors = new List<Player>();
            playerColors.Add(Player.Red);
            playerColors.Add(Player.Blue);
            playerColors.Add(Player.Green);
            playerColors.Add(Player.Yellow);

            for (int i = 0; i < numberOfPlayers; i++)
            {
                GameObject g = Instantiate(player, _spawnPositions[i], new Quaternion(0, 0, 0, 0));

                Material m;

                switch (playerColors[i])
                {
                    case Player.Red:
                        m = redMaterial;
                        break;
                    case Player.Blue:
                        m = blueMaterial;
                        break;
                    case Player.Green:
                        m = greenMaterial;
                        break;
                    case Player.Yellow:
                        m = yellowMaterial;
                        break;
                    default:
                        m = redMaterial;
                        break;
                }

                _occupiedPositions[i] = _spawnPositions[i];
                PlayerController p = g.GetComponent<PlayerController>();

                p.SetCamera(Camera.main);
                p.SetGameHandler(this);
                p.SetPlayer(m);
                _players.Add(p);
            }
        }

        private void CheckIfGameIsDone(int nrOfFinishedPlayers)
        {
            if (nrOfFinishedPlayers >= numberOfPlayers)
            {
                print("Game Done!");
            }
        }

        public void RemovePlayer(PlayerController playerController)
        {
            _players.Remove(playerController);
        }

        public List<PlayerController> GetPlayers()
        {
            return _players;
        }

        private void RemoveBarricadesForInactivePlayers()
        {
            WallController[] wallControllers = (WallController[]) FindObjectsOfType (typeof(WallController));
            TriggerController[] triggerControllers = (TriggerController[]) FindObjectsOfType (typeof(TriggerController));

            foreach (WallController controller in wallControllers)
            {
                if (GetPlayerController(controller.owner) == null)
                {
                    Destroy(controller.gameObject);
                }
            }

            foreach (TriggerController controller in triggerControllers)
            {
                if (GetPlayerController(controller.owner) == null)
                {
                    Destroy(controller.gameObject);
                }
            }
        }

        public Sprite GetSprite(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return upSprite;
                case Direction.Down:
                    return downSprite;
                case Direction.Right:
                    return rightSprite;
                case Direction.Left:
                    return leftSprite;
                case Direction.Blank:
                    return blankSprite;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, "Invalid direction");
            }
        }

        public void AddSequenceObserver(ISequenceObserver iso)
        {
            _sequenceObservers.Add(iso);
        }

        public void NotifySequenceObservers()
        {
            foreach (ISequenceObserver observer in _sequenceObservers)
            {
                observer.GetNotified();
            }
        }

        public void GameProgressUpdate(int nrOfFinishedPlayers)
        {
            CheckIfGameIsDone(nrOfFinishedPlayers);
        }
    }
}