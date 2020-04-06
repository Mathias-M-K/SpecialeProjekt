using System;
using System.Collections;
using System.Collections.Generic;
using Container;
using CoreGame.Interfaces;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
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
        private readonly Dictionary<PlayerTags,Vector2> _occupiedPositions = new Dictionary<PlayerTags, Vector2>();
        private readonly List<PhotonView> _networkedPlayers = new List<PhotonView>();
        public List<PlayerTrade> trades = new List<PlayerTrade>();
        
        //Predefined game variables
        private readonly List<PlayerTags> _playerTags = new List<PlayerTags>(){PlayerTags.Red,PlayerTags.Blue,PlayerTags.Green,PlayerTags.Yellow};
        
        private int numberOfSpawnedPlayers;
        private int numberOfReadyPlayers;
        private int playersFinished;

        public bool IsGameDone { get; private set; }

        //Map information
        private Vector2[] _spawnPositions;

        //Observers
        private readonly List<ISequenceObserver> _sequenceObservers = new List<ISequenceObserver>();
        private readonly List<ITradeObserver> _tradeObservers = new List<ITradeObserver>();
        private readonly List<IFinishPointObserver> _gameProgressObservers = new List<IFinishPointObserver>();


        [Header("Player Prefab")] 
        public GameObject playerPrefab;
        
        [Space] [Header("Game Settings")] 
        [Range(1, 6)] public int numberOfPlayers;
        [Range(1,10)] public float delayBetweenMoves;
        public bool playersAreExternallyControlled = true;
        
        [Space] [Header("Player Abilities")] 
        public bool playersCanPhase;

        [Space] [Header("Other")] 
        public ModalWindowManager endScreen;
        
        
        public void Awake()
        {
            current = this;
        }

        public void StartGame(bool Override)
        {
            if (!Override) return;
            
            AddPlayerController(new PlayerController());
            StartGame();
        }
        public void StartGame()
        {
            if (!playersAreExternallyControlled)
            {
                SpawnMaxPlayers();
            }
            else
            {
                if(_players.Count < 1) throw new InvalidOperationException("Can't start game without any players");
            }
            
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
        
        
        //Checks if the given position is occupied by another player
        public bool IsPositionOccupied(Vector2 position)
        {
            foreach (KeyValuePair<PlayerTags,Vector2> occupiedPosition in _occupiedPositions)
            {
                if (position.x == occupiedPosition.Value.x && position.y == occupiedPosition.Value.y)
                {
                    return true;
                }
            }

            return false;
        }
        
        //Lets player announce their new position
        public void RegisterPosition(PlayerTags playerTags, Vector2 position)
        {
            _occupiedPositions[playerTags] = position;
        }

        //Creates new PlayerTrade
        public void NewTrade(Direction direction, int directionIndex, PlayerTags playerTagsReceiving, PlayerTags playerTagsOffering)
        {
            PlayerController playerReceivingController = GetPlayerController(playerTagsReceiving);
            PlayerController playerOfferingController = GetPlayerController(playerTagsOffering);

            List<ITradeObserver> combinedObserverList = new List<ITradeObserver>();
            List<ITradeObserver> offeringObservers = playerOfferingController.GetTradeObservers();
            List<ITradeObserver> receivingObservers = playerReceivingController.GetTradeObservers();

            combinedObserverList.AddRange(_tradeObservers);
            combinedObserverList.AddRange(offeringObservers);
            combinedObserverList.AddRange(receivingObservers);

            PlayerTrade trade = new PlayerTrade(playerTagsOffering, playerTagsReceiving, direction, this, directionIndex, combinedObserverList);

            trades.Add(trade);

            playerReceivingController.AddIncomingTrade(trade);
            playerOfferingController.AddOutgoingTrade(trade);

            trade.NotifyObservers(TradeActions.TradeOffered);
        }

        //Adds a move to the common sequence
        public void AddMoveToSequence(PlayerTags p, Direction d, int index)
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

            playerController.NotifyInventoryObservers();
            NotifySequenceObservers(SequenceActions.NewMoveAdded, playerMove);
        }

        //Removes a move from the common sequence
        public void RemoveMoveFromSequence(StoredPlayerMove move)
        {
            _sequenceMoves.Remove(move);
            NotifySequenceObservers(SequenceActions.MoveRemoved, move);
        }

        //Performs the sequence, by moving the players in their decired directions
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
                PlayerController playerController = GetPlayerController(pm.PlayerTags);

                try
                {
                    playerController.MovePlayer(pm.Direction);
                    NotifySequenceObservers(SequenceActions.MovePerformed,pm);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
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
        
        public PlayerController GetPlayerController(PlayerTags p)
        {
            foreach (var playerController in _players)
            {
                if (playerController.playerTags == p)
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

        //Networked Players
        
        
        //Spawning players
        public void SpawnMaxPlayers()
        {
            for (int i = numberOfSpawnedPlayers; i < numberOfPlayers; i++)
            {
                SpawnNewPlayer();
            }
        }
        
        /// <summary>Class <c>SpawnNewPlayer</c> Spawns a specific player on a preselected position</summary>
        public void SpawnNewPlayer(PlayerTags playerTags)
        {
            //Checks
            if (_players.Count >= numberOfPlayers) throw new InvalidOperationException("Max players have already been reached");
            if (_players.IndexOf(GetPlayerController(playerTags)) != -1) throw new ArgumentException($"player {playerTags} already exist");
            
            //Code
            int spawnNr = _playerTags.IndexOf(playerTags);
            
            Vector3 spawnPosition = new Vector3(_spawnPositions[spawnNr].x, 1.55f, _spawnPositions[spawnNr].y);
            _occupiedPositions[_playerTags[spawnNr]] = _spawnPositions[spawnNr];
                
            GameObject g = Instantiate(playerPrefab, spawnPosition, new Quaternion(0, 0, 0, 0));
            g.name = _playerTags[spawnNr].ToString();
                
            PlayerController p = g.GetComponent<PlayerController>();

            p.SetPlayer(_playerTags[spawnNr]);
            AddPlayerController(p);

            p.AddReadyObserver(this);
            numberOfSpawnedPlayers++;
        }
        
        /// <summary>Class <c>SpawnNewPlayer</c> Spawns the next player in line, and returns said player</summary>
        public PlayerTags SpawnNewPlayer()
        {
            foreach (PlayerTags player in _playerTags)
            {
                if (GetPlayerController(player) == null)
                {
                    SpawnNewPlayer(player);
                    return player;
                }
            }
            
            throw new InvalidOperationException("All players have been spawned");
        }
        
        private void CheckIfGameIsDone()
        {
            if (playersFinished >= numberOfSpawnedPlayers)
            {
                IsGameDone = true;
                endScreen.OpenWindow();
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

        public void NotifyGameProgressObservers(PlayerTags player1)
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