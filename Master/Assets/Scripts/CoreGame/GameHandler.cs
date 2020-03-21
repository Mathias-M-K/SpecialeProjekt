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
        
        [Space] [Header("Game Settings")] 
        [Range(1, 6)] public int numberOfPlayers;
        [Range(1,10)] public float delayBetweenMoves;
        
        private int playersFinished;

        [Space] [Header("Player Abilities")] 
        public bool playersCanPhase;



        private void Awake()
        {
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

            PlayerTrade trade = new PlayerTrade(playerOffering, playerReceiving, direction, this, directionIndex, combinedObserverList);

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

        public IEnumerator PerformSequence()
        {
            foreach (PlayerController player in _players)
            {
                if (!player.Ready)
                {
                    throw new Exception("All players are not ready!");
                }
            }
            
            //Notifying all sequence observers
            NotifySequenceObservers(SequenceActions.SequenceStarted, null);
            
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
            NotifySequenceObservers(SequenceActions.SequenceEnded,null);
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
                g.name = playerTags[i].ToString();
                
                _occupiedPositions[i] = _spawnPositions[i];
                PlayerController p = g.GetComponent<PlayerController>();

                p.SetPlayer(playerTags[i]);
                AddPlayer(p);
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

        public void AddPlayer(PlayerController playerController)
        {
            _players.Add(playerController);
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
                observer.OnSequenceChange(sequenceAction, move);
            }
        }

        public void NotifyGameProgressObservers(Player player1)
        {
            foreach (IFinishPointObserver observer in _gameProgressObservers)
            {
                observer.OnGameProgressUpdate(player1);
            }

            playersFinished++;
            CheckIfGameIsDone();
        }
    }
}