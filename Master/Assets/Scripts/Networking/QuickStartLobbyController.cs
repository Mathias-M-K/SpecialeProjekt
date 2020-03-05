using Photon.Pun;
using Photon.Pun.Demo.Cockpit.Forms;
using Photon.Realtime;
using UnityEngine;

public class QuickStartLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject quickStartBtn;

    [SerializeField] private GameObject quickCancelBtn;

    public int roomSize;

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        quickStartBtn.SetActive(true);
    }

    public void QuickStart()
    {
        quickStartBtn.SetActive(false);
        quickCancelBtn.SetActive(true);

        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Quick start");
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

    public void QuickCancel()
    {
        quickCancelBtn.SetActive(false);
        quickStartBtn.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }
}