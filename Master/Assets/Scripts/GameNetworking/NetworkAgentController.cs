using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using AdminGUI;
using Container;
using CoreGame;
using CoreGame.Interfaces;
using GameGUI;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// ReSharper disable SuggestVarOrType_SimpleTypes

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

        [Header("Player List")] 
        public GameObject listRect;
        public GameObject listElement;

        [Header("Notification")] 
        public NotificationManager playerNotification;
        public NotificationManager hostNotification;

        [Header("Other")] 
        public TextMeshProUGUI readyCounter;
        public StatTracker statTracker;

        private bool _processingNewTradeAction;

        //Reconnect values
        private readonly Dictionary<string,PlayerTags> _playerDictionary = new Dictionary<string, PlayerTags>();
        private bool _gameStarted;
        private PlayerTags _vacantPlayerTag;
        
        private void Start()
        {
            hostCoverPanel.SetActive(PhotonNetwork.IsMasterClient);
            observerCoverPanel.SetActive(GlobalMethods.IsObserver(PhotonNetwork.NickName));
            playerCoverPanel.SetActive(GlobalMethods.GetRole(PhotonNetwork.NickName).Equals("Participant"));
            
            
            _photonView = GetComponent<PhotonView>();
            
            GameHandler.Current.SetNetworkedAgent(this);
            
            GUIEvents.current.OnButtonHit += OnBtnHit;
            GUIEvents.current.OnManualOverride += OnManualOverride;
            GUIEvents.current.OnPlayerDone += OnPlayerDone;
            GUIEvents.current.OnNewRoundStart += OnNewRoundStart;
        }

        private void OnNewRoundStart(int roundnr)
        {
            
            playerNotification.titleObj.text = $"Round {roundnr}";
            playerNotification.descriptionObj.text = "You've got all your moves back!";
            
            playerNotification.OpenNotification();
        }

        public void OnObserverMark(string nickname, float time)
        {
            _photonView.RPC("RPC_OnObserverMark",RpcTarget.MasterClient,nickname,time);
        }
        [PunRPC]
        private void RPC_OnObserverMark(string nickname, float time)
        {
            statTracker.OnObserverMark(nickname,time);
        }

        private void OnPlayerDone(PlayerController playerController)
        {
            if (playerController.playerTag != _playerTag) return;
            playerCoverPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "You have reached the finish point!";
            playerCoverPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Please wait for your teammates to finish the game";
            LeanTween.moveLocalX(playerCoverPanel, 0, 1f).setEase(LeanTweenType.easeInQuad);
        }

        private void OnManualOverride()
        {
            if (GlobalMethods.GetRole(PhotonNetwork.NickName) != "Participant") return;

            LeanTween.moveLocalX(playerCoverPanel, -563, 1f).setEase(LeanTweenType.easeInQuad);
        }

        private void OnBtnHit(Button button)
        {
            if (button.name.Equals("SpawnPlayers"))
            {
                Dictionary<string,PlayerTags> loggerPlayerList = new Dictionary<string, PlayerTags>();    
                
                button.interactable = false;
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    if (GlobalMethods.GetRole(player.NickName) != "Participant") continue;
                    
                    PlayerTags pTag = GameHandler.Current.SpawnNewPlayer();
                    _playerDictionary.Add(player.NickName,pTag);
                    
                    _photonView.RPC("RPC_SetPlayerTag",player,pTag);
                    
                    loggerPlayerList.Add(player.NickName,pTag);
                }

                statTracker.playerList = loggerPlayerList;
                SetPlayerList();
            }
        }

        private void SetPlayerList()
        {
            int playersCount = _playerDictionary.Count;
            
            string[] tags = new string[playersCount];
            string[] names = new string[playersCount];

            int i = 0;
            
            foreach (KeyValuePair<string,PlayerTags> valuePair in _playerDictionary)
            {
                tags[i] = valuePair.Value.ToString();
                names[i] = valuePair.Key;
                i++;
            }
            _photonView.RPC("RPC_SetPlayerList",RpcTarget.All,names,tags);
        }
        [PunRPC]
        public void RPC_SetPlayerList(string[] names, string[] tags)
        {
            bool skipFirst = false;
            foreach (Transform child in listRect.transform)
            {
                if (!skipFirst)
                {
                    skipFirst = true;
                    continue;
                }
                
                Destroy(child.gameObject);
            }
            
            int i = 0;
            foreach (string s in tags)
            {
                GameObject go = Instantiate(listElement, listRect.transform, false);
                
                PlayerTags playerTag = (PlayerTags)Enum.Parse(typeof(PlayerTags), s);
                
                go.GetComponent<PlayerInfoElement>().SetInfo(names[i],playerTag);
                
                i++;
            }
        }
        
        public void SetGameHandler(GameHandler newGameHandler)
        {
            this.gameHandler = newGameHandler;
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
            _gameStarted = true;
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
        
        
        public void OnReadyStateChanged(bool state, PlayerTags player)
        {
            Debug.Log($"Contacting master with new state: {state}");
            photonView.RPC("RPC_OnReadyStateChanged",RpcTarget.MasterClient,state,player);

            Debug.Log($"Contacting observers with new state: {state}");
            foreach (Player player1 in PhotonNetwork.PlayerList)
            {
                if (GlobalMethods.IsObserver(player1.NickName))
                {
                    _photonView.RPC("RPC_Observer_OnReadyStateChanged",player1,state,player);
                }
            }
        }
        [PunRPC]
        public void RPC_OnReadyStateChanged(bool state,PlayerTags player)
        {
            GameHandler.Current.OnReadyStateChanged(state,player);
            gameHandler.OnReadyStateChanged(state,player);
        }
        [PunRPC]
        public void RPC_Observer_OnReadyStateChanged(bool state, PlayerTags player)
        {
            GUIEvents.current.OnPlayerReadyNotify(state,player);
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

            if (PhotonNetwork.IsMasterClient)
            {
                hostNotification.titleObj.text = "Player Left";
                hostNotification.descriptionObj.text = $"{otherPlayer.NickName}|{_playerDictionary[otherPlayer.NickName]} disconnected from the room";
                hostNotification.OpenNotification();

                if (GlobalMethods.GetRole(otherPlayer.NickName) == "Participant")
                {
                    _vacantPlayerTag = _playerDictionary[otherPlayer.NickName];
                    _playerDictionary.Remove(otherPlayer.NickName);
                    
                    statTracker.OnPlayerDisconnect(otherPlayer.NickName,_vacantPlayerTag);
                }
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (!_gameStarted) return;

            statTracker.OnPlayerReconnect(newPlayer.NickName,_vacantPlayerTag);
            StartCoroutine(UpdateNewPlayer(newPlayer));
        }

        private IEnumerator UpdateNewPlayer(Player newPlayer)
        {
            foreach (var player in GameHandler.Current.GetPlayers())
            {
                _photonView.RPC("RPC_SpawnNewPlayer",newPlayer);
                yield return new WaitForSeconds(0.5f);
            }
            _playerDictionary.Add(newPlayer.NickName,_vacantPlayerTag);
            _photonView.RPC("RPC_SetPlayerTag",newPlayer,_vacantPlayerTag);
            yield return new WaitForSeconds(0.5f);
            _photonView.RPC("RPC_StartGame",newPlayer);

            //Sending gate states
            GameObject mapObj = MapManager.Current.mapObj;
            Transform gates = mapObj.transform.GetChild(1);

            bool[] states = new bool[gates.childCount];
            int j = 0;
            foreach (Transform gate in gates)
            {
                states[j] = gate.GetComponent<GateController>().open;
                j++;
            }
            _photonView.RPC("RPC_SetGateStates",newPlayer,states);
            
            yield return new WaitForSeconds(0.5f);
            
            //Sending current player positions
            Vector2[] positions = new Vector2[gameHandler.GetPlayers().Count];
            int i = 0;
            foreach (PlayerController player in GameHandler.Current.GetPlayers())
            {
                positions[i] = player.GetPosition();
                i++;
            }
            _photonView.RPC("RPC_SetPlayerPositions",newPlayer,positions);
            
            SetPlayerList();
        }
        
        [PunRPC]
        public void RPC_SetGateStates(bool[] states)
        {
            GameObject mapObj = MapManager.Current.mapObj;
            Transform gates = mapObj.transform.GetChild(1);
            int i = 0;
            foreach (Transform gate in gates)
            {
                if (states[i])
                {
                    gate.GetComponent<GateController>().Open();
                }
                i++;
            }
        }

        [PunRPC]
        public void RPC_SetPlayerPositions(Vector2[] positions)
        {
            int i = 0;
            foreach (PlayerController player in gameHandler.GetPlayers())
            {
                player.MoveToPos(positions[i]);
                i++;
            }
        }

        public void SetRoomState(bool roomOpen)
        {
            PhotonNetwork.CurrentRoom.IsOpen = roomOpen;
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