using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using AdminGUI;
using Container;
using CoreGame;
using CoreGame.Interfaces;
using GameGUI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class NetworkAgentController : MonoBehaviourPunCallbacks,IGameHandlerInterface
    {
        private PhotonView _photonView;
        private PlayerTags _playerTag;
        
        [Header("Local Game Handler")]
        public GameHandler gameHandler;

        [Header("Cover Panels")] 
        public GameObject playerCoverPanel;
        public GameObject hostCoverPanel;
        public GameObject observerCoverPanel;

        [Header("Other")] 
        public TextMeshProUGUI readyCounter;

        private bool _processingNewTradeAction;
        
        private void Start()
        {
            hostCoverPanel.SetActive(PhotonNetwork.IsMasterClient);
            observerCoverPanel.SetActive(GlobalMethods.IsObserver(PhotonNetwork.NickName));
            playerCoverPanel.SetActive(GlobalMethods.GetRole(PhotonNetwork.NickName).Equals("Participant"));
            
            
            _photonView = GetComponent<PhotonView>();
            
            GameHandler.Current.SetNetworkedAgent(this);
            
            GUIEvents.current.OnButtonHit += OnBtnHit;
            GUIEvents.current.OnManualOverride += OnManualOverride;
        }

        private void OnManualOverride()
        {
            if (GlobalMethods.GetRole(PhotonNetwork.NickName) != "Participant") return;
            
            LeanTween.moveLocalX(playerCoverPanel, -1920, 1f).setEase(LeanTweenType.easeInQuad);
        }

        private void OnBtnHit(Button button)
        {
            if (button.name.Equals("SpawnPlayers"))
            {
                button.interactable = false;
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    if (GlobalMethods.GetRole(player.NickName) != "Participant") continue;
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

        
        public void UpdateRemoteReadyCounter(string counterText)
        {
            photonView.RPC("RPC_UpdateReadyCounter",RpcTarget.All,counterText);
        }
        [PunRPC]
        public void RPC_UpdateReadyCounter(string counterText)
        {
            readyCounter.text = counterText;
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
            GUIEvents.current.OnManualControl();
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
        /// <param name="moveId"></param>
        /// <param name="index"></param>
        public void AddMoveToSequence(PlayerTags p, Direction d, int moveId, int index)
        {
            photonView.RPC("RPC_AddMoveToSequence",RpcTarget.Others,p,d,moveId,index);
        }
        [PunRPC]
        public void RPC_AddMoveToSequence(PlayerTags p, Direction d, int moveId, int index)
        {
            gameHandler.AddMoveToSequence(p,d,moveId,index);
        }

        
        /// <summary>
        /// RemoveMoveFromSequence()
        /// </summary>
        /// <param name="move"></param>
        public void RemoveMoveFromSequence(StoredPlayerMove move)
        {
            photonView.RPC("RPC_RemoveMoveFromSequence",RpcTarget.Others,move.PlayerTags,move.Direction,move.moveIndex,move.Id);
        }
        [PunRPC]
        public void RPC_RemoveMoveFromSequence(PlayerTags pTag, Direction d,int index, int id)
        {
            StoredPlayerMove storedPMove = new StoredPlayerMove(pTag,d,index,id);
            gameHandler.RemoveMoveFromSequence(storedPMove);
        }

        
        /// <summary>
        /// PerformSequence()
        /// </summary>
        /// <returns></returns>
        public IEnumerator PerformSequence()
        {
            Debug.Log("Calling remote sequences");
            photonView.RPC("RPC_PerformSequence",RpcTarget.All);
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
            if (_processingNewTradeAction)
            {
                _processingNewTradeAction = false;
                return;
            }
            
            photonView.RPC("RPC_OnNewTradeActivity",RpcTarget.Others,trade.TradeID,tradeAction,trade.OfferingPlayerTags,trade.ReceivingPlayerTags,trade.DirectionCounterOffer);
        }
        [PunRPC]
        public void RPC_OnNewTradeActivity(int tradeId,TradeActions tradeAction,PlayerTags offeringPlayer, PlayerTags receivingPlayer, Direction counterMove)
        {
            print($"Received: {tradeId}, {tradeAction}, {offeringPlayer}, {receivingPlayer}, {counterMove}");
            _processingNewTradeAction = true;

            foreach (PlayerTrade trade in gameHandler.GetTrades())
            {
                if (trade.TradeID == tradeId)
                {
                    switch (tradeAction)
                    {
                        case TradeActions.TradeOffered:
                            //Will not happen
                            break;
                        case TradeActions.TradeRejected:
                            trade.RejectTrade(gameHandler.GetPlayerController(receivingPlayer));
                            break;
                        case TradeActions.TradeAccepted:
                            trade.AcceptTrade(counterMove,gameHandler.GetPlayerController(receivingPlayer));
                            break;
                        case TradeActions.TradeCanceled:
                            trade.CancelTrade(offeringPlayer);
                            break;
                        case TradeActions.TradeCanceledByGameHandler:
                            trade.CancelTrade(gameHandler);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(tradeAction), tradeAction, null);
                    }
                    
                    return;
                }
            }
        }
        
        /*
         * Other network related methods
         */
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (otherPlayer.NickName.Equals(GlobalValues.HostTag))
            {
                Disconnect();
            }
        }
        
        
        public void Disconnect()
        {
            GlobalValues.SetConnected(false);
            PhotonNetwork.Disconnect();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            SceneManager.LoadScene(GlobalValues.NetworkScene);
        }

        public void Exit()
        {
            Application.Quit();
        }
        
    }
}