using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Networking
{
    public class LobbyController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject startBtn;

        [SerializeField] private GameObject cancelBtn;

        public int roomSize;

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            startBtn.SetActive(true);
        }

        public void NetworkStart()
        {
            startBtn.SetActive(false);
            cancelBtn.SetActive(true);

            PhotonNetwork.JoinRandomRoom();
            Debug.Log("Start");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("Failed to join a room");
            CreateRoom();
        }

        private void CreateRoom()
        {
            Debug.Log("Creating new room");
            int randomNumber = Random.Range(0, 10000);
            RoomOptions roomOps = new RoomOptions(){IsVisible = true,IsOpen = true,MaxPlayers = (byte) roomSize};
            PhotonNetwork.CreateRoom("Room" + randomNumber, roomOps);
            Debug.Log(randomNumber);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("Failed to create room, trying again");
            CreateRoom();
        }

        public void Cancel()
        {
            cancelBtn.SetActive(false);
            startBtn.SetActive(true);
            PhotonNetwork.LeaveRoom();
        }
    }
}