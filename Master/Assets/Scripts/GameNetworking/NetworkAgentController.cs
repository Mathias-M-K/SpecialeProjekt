using System;
using System.Collections;
using System.Collections.Generic;
using AdminGUI;
using Container;
using CoreGame;
using CoreGame.Interfaces;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class NetworkAgentController : MonoBehaviourPunCallbacks,IGameHandlerInterface
    {
        private PhotonView _photonView;
        private PlayerTags _playerTag;
        public GameHandler gameHandler;

        private bool _processingNewTradeAction;

        private void Start()
        {
            _photonView = GetComponent<PhotonView>();
            //Master task below this line
            GameHandler.Current.SetNetworkedAgent(this);
            GUIEvents.current.OnButtonHit += OnBtnHit;
        }

        private void OnBtnHit(Button button)
        {
            if (button.name.Equals("SpawnPlayers"))
            {
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    _photonView.RPC("RPC_SetPlayerTag",player,GameHandler.Current.SpawnNewPlayer());
                }
            }
        }
        
        public void SetGameHandler(GameHandler gameHandler)
        {
            this.gameHandler = gameHandler;
        }
        [PunRPC]
        public void RPC_SetPlayerTag(PlayerTags playerTag)
        {
            Debug.Log($"Received Player Tag {playerTag}");
            _playerTag = playerTag;
            GUIEvents.current.SetGameTag(playerTag);
        }
        
        
        /// <summary>
        /// Start Game
        /// </summary>
        public void StartGame()
        {
            photonView.RPC("RPC_StartGame",RpcTarget.Others);
        }
        [PunRPC]
        public void RPC_StartGame()
        {
            gameHandler.StartGame();
        }
        
        
        public void SetMapData(MapData mapData)
        {
            photonView.RPC("RPC_SetMapData",RpcTarget.Others,mapData);
        }
        public bool IsPositionOccupied(Vector2 position)
        {
            throw new System.NotImplementedException();
        }
        public void RegisterPosition(PlayerTags playerTags, Vector2 position)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// NewTrade()
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="directionIndex"></param>
        /// <param name="playerTagsReceiving"></param>
        /// <param name="playerTagsOffering"></param>
        /// <param name="tradeId"></param>
        public void NewTrade(Direction direction, int directionIndex, PlayerTags playerTagsReceiving, PlayerTags playerTagsOffering, int tradeId)
        {
            photonView.RPC("RPC_NewTrade",RpcTarget.Others,direction,directionIndex,playerTagsReceiving,playerTagsOffering,tradeId);
        }
        [PunRPC]
        public void RPC_NewTrade(Direction direction, int directionIndex, PlayerTags playerTagsReceiving, PlayerTags playerTagsOffering, int tradeId)
        {
            gameHandler.NewTrade(direction,directionIndex,playerTagsReceiving,playerTagsOffering,tradeId);
        }
        
        
        /*
         * Not Implemented
         */
        public List<PlayerTrade> GetTrades()
        {
            throw new NotImplementedException();
        }
        
        
        /// <summary>
        /// AddMoveToSequence()
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        /// <param name="index"></param>
        public void AddMoveToSequence(PlayerTags p, Direction d, int index)
        {
            photonView.RPC("RPC_AddMoveToSequence",RpcTarget.Others,p,d,index);
        }
        [PunRPC]
        public void RPC_AddMoveToSequence(PlayerTags p, Direction d, int index)
        {
            gameHandler.AddMoveToSequence(p,d,index);
        }

        
        /// <summary>
        /// RemoveMoveFromSequence()
        /// </summary>
        /// <param name="move"></param>
        public void RemoveMoveFromSequence(StoredPlayerMove move)
        {
            photonView.RPC("RPC_RemoveMoveFromSequence",RpcTarget.Others,move);
        }
        [PunRPC]
        public void RPC_RemoveMoveFromSequence(StoredPlayerMove move)
        {
            gameHandler.RemoveMoveFromSequence(move);
        }

        
        /// <summary>
        /// PerformSequence()
        /// </summary>
        /// <returns></returns>
        public IEnumerator PerformSequence()
        {
            Debug.Log("Calling remote sequences");
            photonView.RPC("RPC_PerformSequence",RpcTarget.Others);
            yield break;
        }
        [PunRPC]
        public void RPC_PerformSequence()
        {
            StartCoroutine(gameHandler.PerformSequence());
        }

        
        /*
         * Not Implemented
         */
        public PlayerController GetPlayerController(PlayerTags p)
        {
            throw new System.NotImplementedException();
        }
        public Vector2[] GetSpawnLocations()
        {
            throw new System.NotImplementedException();
        }
        public List<StoredPlayerMove> GetSequence()
        {
            throw new System.NotImplementedException();
        }
        public void SetNetworkedAgent(NetworkAgentController netController)
        {
            throw new System.NotImplementedException();
        }

        
        /// <summary>
        /// SpawnMaxPlayers()
        /// </summary>
        public void SpawnMaxPlayers()
        {
            photonView.RPC("RPC_SpawnMaxPlayers",RpcTarget.Others);
        }
        [PunRPC]
        public void RPC_SpawnMaxPlayers()
        {
            gameHandler.SpawnMaxPlayers();
        }
        
        
        /// <summary>
        /// SpawnNewPlayer(PlayerTag)
        /// </summary>
        /// <param name="playerTag"></param>
        public void SpawnNewPlayer(PlayerTags playerTag)
        {
            photonView.RPC("RPC_SpawnNewPlayer",RpcTarget.Others,playerTag);
        }
        [PunRPC]
        public void RPC_SpawnNewPlayer(PlayerTags playerTag)
        {
            gameHandler.SpawnNewPlayer(playerTag);
        }
        
        
        /// <summary>
        /// SpawnNewPlayer()
        /// </summary>
        public PlayerTags SpawnNewPlayer()
        {
            _photonView.RPC("RPC_SpawnNewPlayer",RpcTarget.Others);
            return PlayerTags.Blank;
        }
        [PunRPC]
        public void RPC_SpawnNewPlayer()
        {
            gameHandler.SpawnNewPlayer();
        }
        

        /*
         * Not Implemented
         */
        public void RemovePlayerController(PlayerController playerController)
        {
            throw new System.NotImplementedException();
        }
        public List<PlayerController> GetPlayers()
        {
            throw new System.NotImplementedException();
        }
        public void AddSequenceObserver(ISequenceObserver iso)
        {
            throw new System.NotImplementedException();
        }
        public void AddTradeObserver(ITradeObserver ito)
        {
            throw new System.NotImplementedException();
        }
        public void AddGameProgressObserver(IFinishPointObserver ifo)
        {
            throw new System.NotImplementedException();
        }
        public void NotifySequenceObservers(SequenceActions sequenceAction, StoredPlayerMove move)
        {
            throw new System.NotImplementedException();
        }
        public void NotifyGameProgressObservers(PlayerTags player1)
        {
            throw new System.NotImplementedException();
        }
        
        
        public void OnReadyStateChanged(bool state)
        {
            Debug.Log($"Contacting master with new state: {state}");
            photonView.RPC("RPC_OnReadyStateChanged",RpcTarget.MasterClient,state);
        }
        [PunRPC]
        public void RPC_OnReadyStateChanged(bool state)
        {
            Debug.Log($"New state received: {state}");
            GameHandler.Current.OnReadyStateChanged(state);
            gameHandler.OnReadyStateChanged(state);
        }

        
        /// <summary>
        /// Communicating new trade updates to all
        /// </summary>
        /// <param name="trade"></param>
        /// <param name="tradeAction"></param>
        public void OnNewTradeActivity(PlayerTrade trade, TradeActions tradeAction)
        {
            /*
             * When i contact the agent and tell it there is a change in one of the trades
             * it will find the trade and apply the change. This will in turn notify the local networkgamehandler,
             * which will contact all players with the new change, which will do the exact same. Basically it will just
             * loop forever. This if statement prevents that
             */
            /*if (_processingNewTradeAction)
            {
                _processingNewTradeAction = false;
                return;
            }*/
            
            photonView.RPC("RPC_OnNewTradeActivity",RpcTarget.Others,trade.TradeID,tradeAction,trade.OfferingPlayerTags,trade.ReceivingPlayerTags,trade.DirectionCounterOffer);
        }
        [PunRPC]
        public void RPC_OnNewTradeActivity(int tradeId,TradeActions tradeAction,PlayerTags offeringPlayer, PlayerTags receivingPlayer, Direction counterMove)
        {
            print($"Received: {tradeId}, {tradeAction}, {offeringPlayer}, {receivingPlayer}, {counterMove}");
            //_processingNewTradeAction = true;
        }
        
        /*
         * Other network related methods
         */

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (otherPlayer.NickName.Equals("Mr. Host"))
            {
                Application.Quit();
            }
        }
    }
}