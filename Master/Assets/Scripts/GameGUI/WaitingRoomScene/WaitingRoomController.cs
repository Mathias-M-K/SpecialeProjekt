using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

namespace GameGUI.WaitingRoomScene
{
    public class WaitingRoomController : MonoBehaviourPunCallbacks
    {
        public WaitingRoomUIController UiController;
        private PhotonView myPhotonView;
        
        public TextMeshProUGUI roomCountDisplay;

        private void Start()
        {
            myPhotonView = GetComponent<PhotonView>();
            UpdateCounter();
            
            foreach (Player player in PhotonNetwork.PlayerList) 
            {
                UiController.AddPlayerToList(player);
            }
            
        }

        private void UpdateCounter()
        {
            roomCountDisplay.text = $"{PhotonNetwork.CurrentRoom.Players.Count}:{PhotonNetwork.CurrentRoom.MaxPlayers}";
        }
        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            print(newPlayer.NickName + " joined");
            
            UpdateCounter();
            UiController.AddPlayerToList(newPlayer);

        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            print(otherPlayer.NickName + " left");

            UpdateCounter();
            UiController.RemovePlayerFromList(otherPlayer);
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
            if (PhotonNetwork.IsMasterClient)
            {
                myPhotonView.RPC("Cancel",RpcTarget.Others);
            }
            
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(GlobalValues.networkScene);
        }
    }
}