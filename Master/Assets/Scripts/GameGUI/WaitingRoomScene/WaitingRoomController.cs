using System;
using System.Collections;
using AdminGUI;
using Michsky.UI.ModernUIPack;
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
        
        [Header("Content")] 
        public GameObject mainContent;

        [Header("Animation Values")]
        public float contentAnimationTime;
        public LeanTweenType contentEaseInType;
        public LeanTweenType contentEaseOutType;
        
        [Header("UI's")] 
        public NotificationManager notification;
        public GameObject hostUi;
        public GameObject playerUi;
        public GameObject mapSelectionMenu;
        [Space]
        public WaitingRoomUIController uiController;
        [Space] 
        public TextMeshProUGUI playerName;
        public TextMeshProUGUI connectionStatus;
        public TextMeshProUGUI roomName;
        public TextMeshProUGUI role;
        public TextMeshProUGUI map;

        private bool _leaving;
        private int _mapIndex = 0;

        private void Awake()
        {
            hostUi.SetActive(PhotonNetwork.IsMasterClient);
            mapSelectionMenu.SetActive(PhotonNetwork.IsMasterClient);
            playerUi.SetActive(!PhotonNetwork.IsMasterClient);
            
        }
        
        private void Start()
        {
            LeanTween.moveLocalX(mainContent, 1236, 0);
            LeanTween.moveLocalX(mainContent, 0, contentAnimationTime).setEase(contentEaseInType);
            
            
            _myPhotonView = GetComponent<PhotonView>();
            UpdatePlayerCounter();

            //Setting info panel text
            playerName.text = GlobalMethods.CleanNickname(PhotonNetwork.NickName);
            connectionStatus.text = "Connected";
            UpdatePlayerCounter();

            role.text = GlobalMethods.GetRole(PhotonNetwork.NickName);
            GlobalValues.SetRole(GlobalMethods.GetRole(PhotonNetwork.NickName));
            
            //Asking for map index
            photonView.RPC("RPC_RequestMapIndex",RpcTarget.MasterClient,PhotonNetwork.LocalPlayer);
            
            foreach (var player in PhotonNetwork.PlayerList) uiController.AddPlayerToList(player);
            
        }

        private void Update()
        {
            //Checking network connection
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                connectionStatus.text = "Disconnected";
                connectionStatus.color = Color.red;

                if (!_leaving)
                {
                    notification.description = "Internet connection lost. Leaving room in 3 seconds";
                    notification.title = "Connection Trouble";
                    notification.OpenNotification();
                    StartCoroutine(DelayedLeave(3));
                    _leaving = true;
                }
            }
        }
        
        
        public void DropdownValueUpdate(int nr)
        {
            _mapIndex = nr;
            map.text = "Map:" + _mapIndex;
            GlobalValues.SetMapIndex(_mapIndex);
            SendMapIndex(_mapIndex,RpcTarget.Others);
        }
        private void SendMapIndex(int index, Player receiver)
        {
            photonView.RPC("RPC_UpdateMapIndex",receiver,index);
        }
        private void SendMapIndex(int index, RpcTarget receiver)
        {
            photonView.RPC("RPC_UpdateMapIndex",receiver,index);
        }
        [PunRPC]
        public void RPC_UpdateMapIndex(int index)
        {
            GlobalValues.SetMapIndex(index);
            map.text = "Map: " + index;
        }
        [PunRPC]
        public void RPC_RequestMapIndex(Player player)
        {
            SendMapIndex(_mapIndex,player);
        }

        
        private IEnumerator DelayedLeave(float delay)
        {
            yield return new WaitForSeconds(delay);
            Cancel();
        }

        private void UpdateMapName()
        {
            map.text = _mapIndex.ToString();
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
            if (otherPlayer.NickName.Equals(GlobalValues.HostTag))
            {
                Cancel();
            }

            UpdatePlayerCounter();
            uiController.RemovePlayerFromList(otherPlayer);
        }
        

        public void StartGame()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            PhotonNetwork.CurrentRoom.IsOpen = false;

            if (GlobalValues.MapIndex == 5)
            {
                PhotonNetwork.LoadLevel(GlobalValues.ChickenScene);
                return;
            }
            PhotonNetwork.LoadLevel(GlobalValues.GameScene);
        }
        
        [PunRPC]
        public void Cancel()
        {
            GlobalValues.SetConnected(false);
            PhotonNetwork.Disconnect();
        }
        
        public override void OnDisconnected(DisconnectCause cause)
        {
            GlobalValues.NetworkSceneFlyInDirection = "left";
            LeanTween.moveLocalX(mainContent, 1920, contentAnimationTime).setEase(contentEaseOutType)
                .setOnComplete(() => { SceneManager.LoadScene(GlobalValues.NetworkScene); });
        }
    }
}