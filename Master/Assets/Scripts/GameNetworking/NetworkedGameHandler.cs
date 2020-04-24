using System;
using System.Collections;
using System.Collections.Generic;
using AdminGUI;
using Container;
using CoreGame;
using CoreGame.Interfaces;
using Photon.Pun;
using UnityEngine;

namespace DefaultNamespace
{
    public class NetworkedGameHandler : GameHandler, ITradeObserver
    {
        public GameHandler localGameHandler;

        public override void Awake()
        {
            GameHandler.Current = this;
        }
        
        private void Start()
        {
            gameObject.AddComponent<GameHandler>();
            localGameHandler.playerPrefab = playerPrefab;
            localGameHandler.numberOfPlayers = numberOfPlayers;
            localGameHandler.delayBetweenMoves = delayBetweenMoves;
            localGameHandler.playersAreExternallyControlled = playersAreExternallyControlled;
            localGameHandler.playersCanPhase = playersCanPhase;
            localGameHandler.endScreen = endScreen;
            
            localGameHandler.AddTradeObserver(this);
        }
        
        
        /*
         * Class specific methods
         */
        public void OnNewTradeActivity(PlayerTrade playerTrade, TradeActions tradeAction)
        {
            if (tradeAction == TradeActions.TradeOffered) return;
            
            MyNetworkedAgent.OnNewTradeActivity(playerTrade,tradeAction);
        }
        public override void SetNetworkedAgent(NetworkAgentController netController)
        {
            MyNetworkedAgent = netController;
        }


        /*
         * Networked
         */
        public override void StartGame()
        {
            MyNetworkedAgent.StartGame();
            localGameHandler.StartGame();

            if (PhotonNetwork.IsMasterClient) UpdateReadyCounter();
        }
        
        
        /*
         * Not Networked
         */
        public override void SetMapData(MapData mapData)
        {
            localGameHandler.SetMapData(mapData);
        }
        public override bool IsPositionOccupied(Vector2 position)
        {
            return localGameHandler.IsPositionOccupied(position);
        }
        public override void RegisterPosition(PlayerTags playerTags, Vector2 position)
        {
            localGameHandler.RegisterPosition(playerTags, position);
        }
        
        
        /*
         * Networked
         */
        public override void NewTrade(Direction direction, int directionIndex, PlayerTags playerTagsReceiving, PlayerTags playerTagsOffering, int tradeId)
        {
            MyNetworkedAgent.NewTrade(direction, directionIndex, playerTagsReceiving, playerTagsOffering, tradeId);
            localGameHandler.NewTrade(direction, directionIndex, playerTagsReceiving, playerTagsOffering,tradeId);
        }
        
        
        /*
         * Not Networked
         */
        public override List<PlayerTrade> GetTrades()
        {
            return localGameHandler.GetTrades();
        }
        
        
        /*
         * Networked
         */
        public override void AddMoveToSequence(PlayerTags p, Direction d,int moveId, int index)
        {
            MyNetworkedAgent.AddMoveToSequence(p, d,moveId, index);
            localGameHandler.AddMoveToSequence(p, d,moveId, index);
        }
        public override void RemoveMoveFromSequence(StoredPlayerMove move)
        {
            MyNetworkedAgent.RemoveMoveFromSequence(move);
            localGameHandler.RemoveMoveFromSequence(move);
        }
        public override IEnumerator PerformSequence()
        {
            yield return StartCoroutine(MyNetworkedAgent.PerformSequence());
            yield return StartCoroutine(localGameHandler.PerformSequence());
        }

        
        /*
         * Not Networked
         */
        public override PlayerController GetPlayerController(PlayerTags p)
        {
            return localGameHandler.GetPlayerController(p);
        }
        public override Vector2[] GetSpawnLocations()
        {
            return localGameHandler.GetSpawnLocations();
        }
        public override List<StoredPlayerMove> GetSequence()
        {
            return localGameHandler.GetSequence();
        }

        /*
         * Networked
         */
        public override void SpawnMaxPlayers()
        {
            MyNetworkedAgent.SpawnMaxPlayers();
            localGameHandler.SpawnMaxPlayers();
        }
        public override void SpawnNewPlayer(PlayerTags playerTag)
        {
            MyNetworkedAgent.SpawnNewPlayer(playerTag);
            localGameHandler.SpawnNewPlayer(playerTag);
        }
        public override PlayerTags SpawnNewPlayer()
        {
            MyNetworkedAgent.SpawnNewPlayer();
            return localGameHandler.SpawnNewPlayer();
        }
        
        /*
         * Not Networked
         */
        public override void RemovePlayerController(PlayerController playerController)
        {
            
            localGameHandler.RemovePlayerController(playerController);
        }
        public override List<PlayerController> GetPlayers()
        {
            return localGameHandler.GetPlayers();
        }
        public override void AddSequenceObserver(ISequenceObserver iso)
        {
            localGameHandler.AddSequenceObserver(iso);
        }
        public override void AddTradeObserver(ITradeObserver ito)
        {
            localGameHandler.AddTradeObserver(ito);
        }
        public override void AddGameProgressObserver(IFinishPointObserver ifo)
        {
            localGameHandler.AddGameProgressObserver(ifo);
        }
        public override void NotifyGameProgressObservers(PlayerTags player1)
        {
            localGameHandler.NotifyGameProgressObservers(player1);
        }
        
        
        /*
         * Networked
         */
        public override void OnReadyStateChanged(bool state, PlayerTags player)
        {

            if (PhotonNetwork.IsMasterClient)
            {
                GUIEvents.current.OnPlayerReadyNotify(state,player);
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

                UpdateReadyCounter();
                
                if (_numberOfReadyPlayers == localGameHandler.numberOfSpawnedPlayers)
                {
                    StartCoroutine(MyNetworkedAgent.PerformSequence());
                    _numberOfReadyPlayers = 0;
                }
            }

            if (!PhotonNetwork.IsMasterClient)
            {
                MyNetworkedAgent.OnReadyStateChanged(state,player);
            }
        }

        private void UpdateReadyCounter()
        {
            MyNetworkedAgent.UpdateRemoteReadyCounter($"{_numberOfReadyPlayers}/{localGameHandler.numberOfSpawnedPlayers} Ready");
        }
    }
}