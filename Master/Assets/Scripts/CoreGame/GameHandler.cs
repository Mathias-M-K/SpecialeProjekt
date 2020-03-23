using System;
using System.Collections;
using System.Collections.Generic;
using Container;
using CoreGame.Interfaces;
using UnityEngine;

// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable PossibleLossOfFraction

namespace CoreGame
{
    public class GameHandler : MonoBehaviour, IReadyObserver
    {
        public static GameHandler current;

        //Game variables
        private readonly List<StoredPlayerMove> _sequenceMoves = new List<StoredPlayerMove>();
        private readonly List<PlayerController> _players = new List<PlayerController>();
        private readonly Vector2[] _occupiedPositions = new Vector2[4];
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
        public bool playersAreExternallyControlled;

        private int numberOfSpawnedPlayers;
        private int numberOfReadyPlayers;
        private int playersFinished;

        //Value for checking if game is done. All IGameEndObservers are automatically updated
        public bool IsGameDone { get; private set; }
        
        [Space] [Header("Player Abilities")] 
        public bool playersCanPhase;
        
        
        public void Awake()
        {
            current = this;
        }

        private void StartGame()
        {
            RemoveBarricadesForInactivePlayers();
        }

        public void SetMapData(MapData mapData)
        {
            _spawnPositions = mapData.spawnPositions;
            
            if (numberOfPlayers > mapData.maxPlayers)
            {
                Debug.LogError($"Number of max players is to high for this map!, map only allows {mapData.maxPlayers} players",
                    this);
                numberOfPlayers = mapData.maxPlayers;
            }
            
        }
        
        public bool IsPositionOccupied(Vector2 position)
        {
            foreach (Vector2 occupiedPosition in _occupiedPositions)
            {
                if (position.x == occupiedPosition.x && position.y == occupiedPosition.y)
                {
                    return true;
                }
            }

            return false;
        }

        //Lets players announce their position
        public void RegisterPosition(Player player, Vector3 position)
        {
            int index = 0;
            switch (player)
            {
                case Player.Red:
                    index = 0;
                    break;
                case Player.Blue:
                    index = 1;
                    break;
                case Player.Green:
                    index = 2;
                    break;
                case Player.Yellow:
                    index = 3;
                    break;
                default:
                    throw new ArgumentException("Not a valid player");
            }
            _occupiedPositions[index] = new Vector2(position.x,position.z);
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
                throw new ArgumentException(p + " is not active");
            }

            if (playerController.GetIndexForDirection(d) == -1)
            {
                throw new InvalidOperationException($"{playerPrefab} does not posses the {d} move");
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

                try
                {
                    playerController.MovePlayer(pm.Direction);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
                yield return new WaitForSeconds(delayBetweenMoves);
            }

            foreach (PlayerTrade trade in trades)
            {
                trade.CancelTrade(this);
            }

            foreach (PlayerController playerController in _players)
            {
                playerController.ResetAfterSequence();
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

        //Spawning players
        public void SpawnMaxPlayers()
        {
            List<Player> playerTags = new List<Player>(){Player.Red,Player.Blue,Player.Green,Player.Yellow};

            for (int i = numberOfSpawnedPlayers; i < numberOfPlayers; i++)
            {
                SpawnNewPlayer();
            }
        }
        public Player SpawnNewPlayer()
        {
            if (_players.Count >= numberOfPlayers)
            {
                throw new InvalidOperationException("Max players have already been reached");
            }
            
            List<Player> playerTags = new List<Player>(){Player.Red,Player.Blue,Player.Green,Player.Yellow};

            int spawnNr = _players.Count;

            Vector3 spawnPosition = new Vector3(_spawnPositions[spawnNr].x, 1.55f, _spawnPositions[spawnNr].y);
            _occupiedPositions[spawnNr] = _spawnPositions[spawnNr];
                
            GameObject g = Instantiate(playerPrefab, spawnPosition, new Quaternion(0, 0, 0, 0));
            g.name = playerTags[spawnNr].ToString();
                
            PlayerController p = g.GetComponent<PlayerController>();

            p.SetPlayer(playerTags[spawnNr]);
            AddPlayerController(p);

            p.AddReadyObserver(this);
            numberOfSpawnedPlayers++;
            
            return p.player;
        }
        
        private void CheckIfGameIsDone()
        {
            if (playersFinished >= numberOfSpawnedPlayers)
            {
                IsGameDone = true;
            }
        }

        public void RemovePlayerController(PlayerController playerController)
        {
            _players.Remove(playerController);
        }

        private void AddPlayerController(PlayerController playerController)
        {
            _players.Add(playerController);
        }

        public List<PlayerController> GetPlayers()
        {
            return _players;
        }

        private void RemoveBarricadesForInactivePlayers()
        {
            GateController[] wallControllers = (GateController[]) FindObjectsOfType(typeof(GateController));
            TriggerController[] triggerControllers = (TriggerController[]) FindObjectsOfType(typeof(TriggerController));

            foreach (GateController controller in wallControllers)
            {
                if (GetPlayerController(controller.Owner) == null)
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

        public void OnReadyStateChanged(bool state)
        {
            switch (state)  
            {
                case true:
                    numberOfReadyPlayers++;
                    break;
                case false:
                    numberOfReadyPlayers--;
                    break;
            }

            if (numberOfReadyPlayers == numberOfSpawnedPlayers)
            {
                StartCoroutine(PerformSequence());
            }
        }
    }
}