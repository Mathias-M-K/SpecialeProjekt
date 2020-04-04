using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameGUI.WaitingRoomScene
{
    public class WaitingRoomController : MonoBehaviourPunCallbacks
    {
        private PhotonView _myPhotonView;

        
        
        [Header("UI's")] 
        public GameObject hostUi;
        public GameObject playerUi;
        [Space]
        public WaitingRoomUIController uiController;
        [Space] 
        public TextMeshProUGUI playerName;
        public TextMeshProUGUI connectionStatus;
        public TextMeshProUGUI roomName;
        public TextMeshProUGUI role;

        private void Awake()
        {
            hostUi.SetActive(PhotonNetwork.IsMasterClient);
            playerUi.SetActive(!PhotonNetwork.IsMasterClient);
        }
        
        private void Start()
        {
            _myPhotonView = GetComponent<PhotonView>();
            UpdatePlayerCounter();

            //Setting infopanel text
            playerName.text = PhotonNetwork.NickName;
            connectionStatus.text = "Connected";
            UpdatePlayerCounter();

            if (PhotonNetwork.IsMasterClient)
            {
                role.text = "Role: Host";
            }
            else
            {
                role.text = "Role: Participant";
            }
            
            foreach (var player in PhotonNetwork.PlayerList) uiController.AddPlayerToList(player);
            
        }

        private void UpdatePlayerCounter()
        {
            roomName.text = $"{PhotonNetwork.CurrentRoom.Name} | {PhotonNetwork.CurrentRoom.Players.Count}:{PhotonNetwork.CurrentRoom.MaxPlayers}";
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            print(newPlayer.NickName + " joined");

            UpdatePlayerCounter();
            uiController.AddPlayerToList(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            print(otherPlayer.NickName + " left");

            UpdatePlayerCounter();
            uiController.RemovePlayerFromList(otherPlayer);
        }
        
        public override void OnDisconnected(DisconnectCause cause)
        {
            connectionStatus.text = "Disconnected";
            PhotonNetwork.ReconnectAndRejoin();
        }
        
        

        public void StartGame()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(GlobalValues.gameScene);
        }

        [PunRPC]
        public void Cancel()
        {
            if (PhotonNetwork.IsMasterClient) _myPhotonView.RPC("Cancel", RpcTarget.Others);

            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(GlobalValues.networkScene);
        }
    }
}