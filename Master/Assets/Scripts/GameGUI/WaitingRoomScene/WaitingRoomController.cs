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

        public TextMeshProUGUI roomCountDisplay;
        public WaitingRoomUIController uiController;
        
        [Header("UI's")] 
        public GameObject hostUi;
        public GameObject playerUi;

        private void Awake()
        {
            hostUi.SetActive(PhotonNetwork.IsMasterClient);
            playerUi.SetActive(!PhotonNetwork.IsMasterClient);
        }
        
        private void Start()
        {
            _myPhotonView = GetComponent<PhotonView>();
            UpdateCounter();

            foreach (var player in PhotonNetwork.PlayerList) uiController.AddPlayerToList(player);
        }

        private void UpdateCounter()
        {
            roomCountDisplay.text = $"{PhotonNetwork.CurrentRoom.Players.Count}:{PhotonNetwork.CurrentRoom.MaxPlayers}";
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            print(newPlayer.NickName + " joined");

            UpdateCounter();
            uiController.AddPlayerToList(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            print(otherPlayer.NickName + " left");

            UpdateCounter();
            uiController.RemovePlayerFromList(otherPlayer);
        }


        /*public override void OnDisconnected(DisconnectCause cause)
        {
            
        }*/

        public override void OnDisconnected(DisconnectCause cause)
        {
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