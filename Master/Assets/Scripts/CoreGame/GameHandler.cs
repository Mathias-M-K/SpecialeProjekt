using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdminGUI;
using Container;
using CoreGame.Interfaces;
using DefaultNamespace;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Analytics;
using Random = UnityEngine.Random;

namespace CoreGame
{
    public class GameHandler : MonoBehaviour, IReadyObserver, IGameHandlerInterface
    {
        public static GameHandler Current;
        public virtual void Awake()
        {
            if (GameHandler.Current != null) return;
            Current = this;
        }
        
        //Game variables
        private readonly List<StoredPlayerMove> _sequenceMoves = new List<StoredPlayerMove>();
        private readonly List<PlayerController> _players = new List<PlayerController>();
        private readonly Dictionary<PlayerTags,Vector2> _occupiedPositions = new Dictionary<PlayerTags, Vector2>();
        //private readonly List<PhotonView> _networkedPlayers = new List<PhotonView>();
        public List<PlayerTrade> trades = new List<PlayerTrade>();
        
        //Predefined game variables
        private readonly List<PlayerTags> _playerTags = new List<PlayerTags>();

        private void Start()
        {
            foreach(PlayerTags playerTag in Enum.GetValues(typeof(PlayerTags)))
            {
                _playerTags.Add(playerTag);
            }
        }

        private int _nrOfPlayersAtGameStart;
        public int numberOfSpawnedPlayers;
        private protected int _numberOfReadyPlayers;
        private int _playersFinished = 0;

        public bool isGameDone { get; private set; }

        //Map information
        private Vector2[] _spawnPositions;

        //Observers
        private readonly List<ISequenceObserver> _sequenceObservers = new List<ISequenceObserver>();
        private readonly List<ITradeObserver> _tradeObservers = new List<ITradeObserver>();
        private readonly List<IFinishPointObserver> _gameProgressObservers = new List<IFinishPointObserver>();
        
        //Networked Agent
        protected NetworkAgentController MyNetworkedAgent;


        [Header("Player Prefab")] 
        public GameObject playerPrefab;
        
        [Space] [Header("Game Settings")] 
        [Range(1, 16)] public int numberOfPlayers;
        [Range(0,10)] public float delayBetweenMoves;
        public bool playersAreExternallyControlled = true;
        
        [Space] [Header("Player Abilities")] 
        public bool playersCanPhase;

        [Space] [Header("Other")] 
        public ModalWindowManager endScreen;

        /// <summary>
        /// starts game
        /// </summary>
        public virtual void StartGame()
        {
            if (!playersAreExternallyControlled)
            {
                SpawnMaxPlayers();
            }
            else
            {
                if(_players.Count < 1) throw new InvalidOperationException("Can't start game without any players");
            }

            _nrOfPlayersAtGameStart = numberOfSpawnedPlayers;

            
            RemoveBarricadesForInactivePlayers();
        }

        /// <summary>
        /// Sets spawn positions and max allowed players
        /// </summary>
        /// <param name="mapData"></param>
        public virtual void SetMapData(MapData mapData)
        {
            _spawnPositions = mapData.spawnPositions;
            
            if (numberOfPlayers > mapData.maxPlayers)
            {
                Debug.Log($"Number of max players is to high for this map!, map only allows {mapData.maxPlayers} players",
                    this);
                numberOfPlayers = mapData.maxPlayers;
            }
            
        }

        /// <summary>
        /// Checks if the given position is occupied by another player
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public virtual bool IsPositionOccupied(Vector2 position)
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

        /// <summary>
        /// Lets player announce their new position
        /// </summary>
        /// <param name="playerTags"></param>
        /// <param name="position"></param>
        public virtual void RegisterPosition(PlayerTags playerTags, Vector2 position)
        {
            _occupiedPositions[playerTags] = position;
        }

        /// <summary>
        /// Creates new PlayerTrade
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="directionIndex"></param>
        /// <param name="playerTagsReceiving"></param>
        /// <param name="playerTagsOffering"></param>
        /// <param name="tradeId"></param>
        public virtual void NewTrade(Direction direction, int directionIndex, PlayerTags playerTagsReceiving, PlayerTags playerTagsOffering, int tradeId)
        {
            PlayerController playerReceivingController = GetPlayerController(playerTagsReceiving);
            PlayerController playerOfferingController = GetPlayerController(playerTagsOffering);

            List<ITradeObserver> combinedObserverList = new List<ITradeObserver>();
            List<ITradeObserver> offeringObservers = playerOfferingController.GetTradeObservers();
            List<ITradeObserver> receivingObservers = playerReceivingController.GetTradeObservers();

            combinedObserverList.AddRange(_tradeObservers);
            combinedObserverList.AddRange(offeringObservers);
            combinedObserverList.AddRange(receivingObservers);

            PlayerTrade trade = new PlayerTrade(playerTagsOffering, playerTagsReceiving, direction, this, directionIndex, combinedObserverList,tradeId);

            trades.Add(trade);

            playerReceivingController.AddIncomingTrade(trade);
            playerOfferingController.AddOutgoingTrade(trade);
            playerOfferingController.RemoveMove(directionIndex);

            trade.NotifyObservers(TradeActions.TradeOffered);
        }

        /// <summary>
        /// Returns all current playerTrades
        /// </summary>
        /// <returns></returns>
        public virtual List<PlayerTrade> GetTrades()
        {
            return trades;
        }

        /// <summary>
        /// Adds a move to the common sequence
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <param name="index"></param>
        public virtual void AddMoveToSequence(PlayerTags p, Direction d,int moveId, int index)
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
            
            StoredPlayerMove playerMove = new StoredPlayerMove(p, d, index, moveId);
            _sequenceMoves.Add(playerMove);

            playerController.RemoveMove(index);

            playerController.NotifyInventoryObservers();
            NotifySequenceObservers(SequenceActions.NewMoveAdded, playerMove);
        }

        /// <summary>
        /// Removes a move from the common sequence
        /// </summary>
        /// <param name="move"></param>
        public virtual void RemoveMoveFromSequence(StoredPlayerMove move)
        {
            StoredPlayerMove currentMove;
            
            try
            {
                currentMove = _sequenceMoves.First(item => item.Id == move.Id);
            }
            catch(Exception e)
            {
                Debug.Log($"RemoveFromSequence Error: {e}");
                return;
            }
            
            _sequenceMoves.Remove(currentMove);
            
            GetPlayerController(move.PlayerTags).AddMove(move.Direction,move.moveIndex);
            NotifySequenceObservers(SequenceActions.MoveRemoved, currentMove);
        }

        /// <summary>
        /// Performs the sequence, by moving the players in their desired directions
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator PerformSequence()
        {
            /*
            foreach (PlayerController player in _players)
            {
                if (!player.Ready)
                {
                    throw new Exception("All players are not ready!");
                }
            }*/
            
            //Notifying all sequence observers
            NotifySequenceObservers(SequenceActions.SequenceStarted, null);
            
            List<StoredPlayerMove> tempSequence = new List<StoredPlayerMove>(_sequenceMoves);
            foreach (StoredPlayerMove pm in tempSequence)
            {
                PlayerController playerController = GetPlayerController(pm.PlayerTags);

                try
                {
                    if (playerController == null)
                    {
                        Debug.Log("Player does not exist, continuing");
                        continue;
                    }

                    playerController.MovePlayer(pm.Direction);
                    NotifySequenceObservers(SequenceActions.MovePerformed,pm);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                yield return new WaitForSeconds(delayBetweenMoves);
            }

            List<PlayerTrade> tempTrades = new List<PlayerTrade>(trades);
            foreach (PlayerTrade trade in tempTrades)
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

        /// <summary>
        /// Gets a specific playercontroller based on the playertag.
        /// </summary>
        /// <param name="p"></param>
        /// <returns>PlayerController</returns>
        public virtual PlayerController GetPlayerController(PlayerTags p)
        {
            foreach (var playerController in _players)
            {
                if (playerController.playerTag == p)
                {
                    return playerController;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the predefined spawn positions that are dictated by the mapdata file
        /// </summary>
        /// <returns></returns>
        public virtual Vector2[] GetSpawnLocations()
        {
            return _spawnPositions;
        }

        /// <summary>
        /// Returns all moves that are currently in que to be played
        /// </summary>
        /// <returns>Player moves</returns>
        public virtual List<StoredPlayerMove> GetSequence()
        {
            return _sequenceMoves;
        }

        /// <summary>
        /// When the locally owned photonview spawns, it sets itself as the networked agent
        /// </summary>
        /// <param name="netController"></param>
        public virtual void SetNetworkedAgent(NetworkAgentController netController)
        {
            MyNetworkedAgent = netController;
        }

        /// <summary>
        /// Spawn the max amount of players that the mapdata dictates are allowed
        /// </summary>
        public virtual void SpawnMaxPlayers()
        {
            for (int i = numberOfSpawnedPlayers; i < numberOfPlayers; i++)
            {
                SpawnNewPlayer();
            }
        }

        /// <summary>
        /// Spawns a specific player on a preselected position
        /// </summary>
        /// <param name="playerTag"></param>
        public virtual void SpawnNewPlayer(PlayerTags playerTag)
        {
            //Checks
            if (_players.Count >= numberOfPlayers) throw new InvalidOperationException("Max players have already been reached");
            if (_players.IndexOf(GetPlayerController(playerTag)) != -1) throw new ArgumentException($"player {playerTag} already exist");
            
            //Code
            int spawnNr = _playerTags.IndexOf(playerTag);
            
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

        /// <summary>
        /// Spawns the next player in line, and returns said player
        /// </summary>
        /// <returns>Spawned player</returns>
        public virtual PlayerTags SpawnNewPlayer()
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

        /// <summary>
        /// Checks if the game is done
        /// </summary>
        private void CheckIfGameIsDone()
        {
            if (_playersFinished >= _nrOfPlayersAtGameStart)
            {
                isGameDone = true;
                endScreen.OpenWindow();
            }
        }

        /// <summary>
        /// Removes a player controller from _players list
        /// </summary>
        /// <param name="playerController"></param>
        public virtual void RemovePlayerController(PlayerController playerController)
        {
            _players.Remove(playerController);
            numberOfSpawnedPlayers--;
        }

        private void AddPlayerController(PlayerController playerController)
        {
            _players.Add(playerController);
        }

        /// <summary>
        /// Returns all active player controllers
        /// </summary>
        /// <returns></returns>
        public virtual List<PlayerController> GetPlayers()
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
        
        
        /**
         * Add and notify methods
         */
        public virtual void AddSequenceObserver(ISequenceObserver iso)
        {
            _sequenceObservers.Add(iso);
        }

        public virtual void AddTradeObserver(ITradeObserver ito)
        {
            _tradeObservers.Add(ito);
        }

        public virtual void AddGameProgressObserver(IFinishPointObserver ifo)
        {
            _gameProgressObservers.Add(ifo);
        }

        public void NotifySequenceObservers(SequenceActions sequenceAction, StoredPlayerMove move)
        {
            foreach (ISequenceObserver observer in _sequenceObservers)
            {
                observer.OnSequenceChange(sequenceAction, move);
            }
        }

        public virtual void NotifyGameProgressObservers(PlayerTags player1)
        {
            foreach (IFinishPointObserver observer in _gameProgressObservers)
            {
                observer.OnGameProgressUpdate(player1);
            }
            
            _playersFinished++;
            CheckIfGameIsDone();
        }

        public virtual void OnReadyStateChanged(bool state, PlayerTags player)
        {
            switch (state)  
            {
                case true:
                    _numberOfReadyPlayers++;
                    break;
                case false:
                    _numberOfReadyPlayers--;
                    break;
            }
            
            /*
             * This is a stupid network fix. Basically, all client will report back to
             * the master and say they are not ready. At the same time, the master itself will also set
             * players to not be ready. The result is that the master thinks that -4 players are ready
             * after a sequence have been played, which should be 0.
             */

            if (_numberOfReadyPlayers < 0) _numberOfReadyPlayers = 0;

            /*if (_numberOfReadyPlayers == numberOfSpawnedPlayers)
            {
                Debug.Log("Performing sequence on local gamehandler");
                StartCoroutine(PerformSequence());
            }*/
        }
    }
}