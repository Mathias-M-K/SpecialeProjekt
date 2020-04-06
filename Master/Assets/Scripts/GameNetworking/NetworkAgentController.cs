using AdminGUI;
using CoreGame;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class NetworkAgentController : MonoBehaviourPunCallbacks
    {
        private PhotonView _photonView;
        private PlayerTags _playerTag;
        private GameHandler _gameHandler;

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
            _gameHandler = gameHandler;
        }

        [PunRPC]
        public void RPC_SetPlayerTag(PlayerTags playerTag)
        {
            Debug.Log($"Received Player Tag {playerTag}");
            _playerTag = playerTag;
            GUIEvents.current.SetGameTag(playerTag);
        }


        public void SpawnNewPlayer()
        {
            _photonView.RPC("RPC_SpawnPlayer",RpcTarget.Others);
        }

        [PunRPC]
        public void RPC_SpawnPlayer()
        {
            print("RPC_SPAWNPLAYER");
            //_gameHandler.SpawnNewPlayer();
        }


    }
}