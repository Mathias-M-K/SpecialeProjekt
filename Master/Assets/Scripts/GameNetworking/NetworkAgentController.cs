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

        private void Start()
        {
            _photonView = GetComponent<PhotonView>();
            
            if (!_photonView.IsMine) return;
            
            //Master task below this line
            GUIEvents.current.onButtonHit += OnBtnHit;
        }

        private void OnBtnHit(Button button)
        {
            print("HEy");
            if (button.name.Equals("SpawnPlayers"))
            {
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    _photonView.RPC("RPC_SetPlayerTag",player,GameHandler.current.SpawnNewPlayer());
                }
            }
        }

        [PunRPC]
        public void RPC_SetPlayerTag(PlayerTags playerTag)
        {
            print("RPC_SetPlayer Activated");
            _playerTag = playerTag;
            Debug.LogError(_playerTag);
        }


    }
}