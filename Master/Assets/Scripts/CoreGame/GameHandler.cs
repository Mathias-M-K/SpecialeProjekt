using System;
using System.Collections;
using System.Collections.Generic;
using Container;
using CoreGame.Interfaces;
using UnityEngine;
using UnityEngine.AI;

// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable PossibleLossOfFraction

namespace CoreGame
{
    public class GameHandler : MonoBehaviour
    {
        public static GameHandler current;

        //Game variables
        private readonly List<StoredPlayerMove> _sequenceMoves = new List<StoredPlayerMove>();
        private readonly List<PlayerController> _players = new List<PlayerController>();
        private readonly Vector3[] _occupiedPositions = new Vector3[4];
        public List<PlayerTrade> trades = new List<PlayerTrade>();

        //Map information
        private Vector2[] _spawnPositions;

        //Observers
        private readonly List<ISequenceObserver> _sequenceObservers = new List<ISequenceObserver>();
        private readonly List<ITradeObserver> _tradeObservers = new List<ITradeObserver>();
        private readonly List<IFinishPointObserver> _gameProgressObservers = new List<IFinishPointObserver>();


        [Header("Player Prefab")] public GameObject playerPrefab;



        [Space] [Header("Settings")] [Range(1, 6)]
        public int numberOfPlayers;

        [SerializeField] private int playersFinished;

        [Space] [Header("Player Abilities")] public bool playersCanPhase;

        [Space] [Header("Materials")] 
        public Material redMaterial;
        public Material blueMaterial;
        public Material greenMaterial;
        public Material yellowMaterial;

        [Space] [Header("Sprites")] 
        public Sprite leftSprite;
        public Sprite rightSprite;
        public Sprite upSprite;
        public Sprite downSprite;
        public Sprite blankSprite;
        public Sprite arrowSprite;

        private void Awake()
        {
            print("did it");
            current = this;

            
        }

        private void Start()
        {

            RemoveBarricadesForInactivePlayers();

        }

        public void InitializeGame(MapData mapData)
        {
            _spawnPositions = mapData.spawnPositions;
            SpawnMaxPlayers(mapData.maxPlayers);
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

            List<ITradeObserver> combinedObserverList = new List<ITradeObserver>();
            List<ITradeObserver> offeringObservers = playerOfferingController.GetTradeObservers();
            List<ITradeObserver> receivingObservers = playerReceivingController.GetTradeObservers();

            combinedObserverList.AddRange(_tradeObservers);
            combinedObserverList.AddRange(offeringObservers);
            combinedObserverList.AddRange(receivingObservers);

            PlayerTrade trade = new PlayerTrade(playerOffering, playerReceiving, direction, this, directionIndex,
                combinedObserverList);

            trades.Add(trade);

            playerReceivingController.AddIncomingTrade(trade);
            playerOfferingController.AddOutgoingTrade(trade);

            trade.NotifyObservers(TradeActions.TradeOffered);
        }

        public void AddMoveToSequence(Player p, Direction d, int index)
        {
            PlayerController playerController = GetPlayerController(p);

            if (playerController == null)
            {
                Debug.LogException(new ArgumentException(p + " is not active"), this);
                return;
            }

            if (playerController.GetIndexForDirection(d) == -1)
            {
                Debug.LogError($"{playerPrefab} does not posses the {d} move");
                return;
            }

            if (d == Direction.Blank) throw new ArgumentException("Can't add blank moves");

            StoredPlayerMove playerMove = new StoredPlayerMove(p, d);
            _sequenceMoves.Add(playerMove);

            playerController.RemoveMove(index);

            playerController.NotifyMoveObservers();
            NotifySequenceObservers(SequenceActions.NewMoveAdded, playerMove);
        }

        public void RemoveMoveFromSequence(StoredPlayerMove move)
        {
            _sequenceMoves.Remove(move);
            NotifySequenceObservers(SequenceActions.MoveRemoved, move);
        }

        public IEnumerator PerformSequence(float delayBetweenMoves)
        {
            foreach (StoredPlayerMove pm in _sequenceMoves)
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
            NotifySequenceObservers(SequenceActions.SequencePlayed, null);
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

        public Vector2[] GetSpawnLocations()
        {
            return _spawnPositions;
        }

        public List<StoredPlayerMove> GetSequence()
        {
            return _sequenceMoves;
        }

        //Spawning x player, where x is the number of allowed players
        private void SpawnMaxPlayers(int mapPlayerLimit)
        {
            if (numberOfPlayers > mapPlayerLimit)
            {
                Debug.LogError($"Number of max players have been exceeded, fallback to {mapPlayerLimit} players",
                    this);
                numberOfPlayers = mapPlayerLimit;
            }

            List<Player> playerTags = new List<Player>();
            playerTags.Add(Player.Red);
            playerTags.Add(Player.Blue);
            playerTags.Add(Player.Green);
            playerTags.Add(Player.Yellow);

            for (int i = 0; i < numberOfPlayers; i++)
            {
                Vector3 spawnPosition = new Vector3(_spawnPositions[i].x, 1.55f, _spawnPositions[i].y);

                GameObject g = Instantiate(playerPrefab, spawnPosition, new Quaternion(0, 0, 0, 0));

                Material m = GetPlayerMaterial(playerTags[i]);
                
                _occupiedPositions[i] = _spawnPositions[i];
                PlayerController p = g.GetComponent<PlayerController>();

                p.SetCamera(Camera.main);
                p.SetGameHandler(this);
                p.SetPlayer(m);
                _players.Add(p);
            }
        }

        

        private void CheckIfGameIsDone()
        {
            if (playersFinished >= numberOfPlayers)
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
            WallController[] wallControllers = (WallController[]) FindObjectsOfType(typeof(WallController));
            TriggerController[] triggerControllers = (TriggerController[]) FindObjectsOfType(typeof(TriggerController));

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

        public int GetDirectionRotation(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return 90;
                case Direction.Down:
                    return -90;
                case Direction.Right:
                    return 0;
                case Direction.Left:
                    return 180;
                case Direction.Blank:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, "Invalid direction");
            }
        }
        
        public Material GetPlayerMaterial(Player p)
        {
            switch (p)
            {
                case Player.Red:
                    return redMaterial;
                case Player.Blue:
                    return blueMaterial;
                case Player.Green:
                    return greenMaterial;
                case Player.Yellow:
                    return yellowMaterial;
                default:
                    return redMaterial;
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


        //Add and notify methods for observers

        public void AddSequenceObserver(ISequenceObserver iso)
        {
            _sequenceObservers.Add(iso);
        }

        public void AddTradeObserver(ITradeObserver ito)
        {
            _tradeObservers.Add(ito);
        }

        public void AddGameProgressObserver(IFinishPointObserver ifo)
        {
            _gameProgressObservers.Add(ifo);
        }

        private void NotifySequenceObservers(SequenceActions sequenceAction, StoredPlayerMove move)
        {
            foreach (ISequenceObserver observer in _sequenceObservers)
            {
                observer.SequenceUpdate(sequenceAction, move);
            }
        }

        public void NotifyGameProgressObservers(Player player1)
        {
            foreach (IFinishPointObserver observer in _gameProgressObservers)
            {
                observer.GameProgressUpdate(player1);
            }

            playersFinished++;
            CheckIfGameIsDone();
        }
    }
}