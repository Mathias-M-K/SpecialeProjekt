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

        private void Start()
        {
            _photonView = GetComponent<PhotonView>();
            
            if (!_photonView.IsMine) return;
            
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
        public void NewTrade(Direction direction, int directionIndex, PlayerTags playerTagsReceiving, PlayerTags playerTagsOffering)
        {
            photonView.RPC("RPC_NewTrade",RpcTarget.Others,direction,directionIndex,playerTagsReceiving,playerTagsOffering);
        }
        [PunRPC]
        public void RPC_NewTrade(Direction direction, int directionIndex, PlayerTags playerTagsReceiving, PlayerTags playerTagsOffering)
        {
            gameHandler.NewTrade(direction,directionIndex,playerTagsReceiving,playerTagsOffering);
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
            photonView.RPC("RPC_OnReadyStateChanged",RpcTarget.Others,state);
        }
        [PunRPC]
        public void RPC_OnReadyStateChanged(bool state)
        {
            gameHandler.OnReadyStateChanged(state);
        }
    }
}