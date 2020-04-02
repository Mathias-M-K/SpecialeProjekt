using GameGUI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Networking
{
    public class WaitingRoomController : MonoBehaviourPunCallbacks
    {
        private PhotonView myPhotonView;
        
        private int playerCount;
        private int roomSize;

        public int minPlayersToStart;

        public TextMeshProUGUI roomCountDisplay;
        public TextMeshProUGUI timerToStartDisplay;

        private bool readyToCountDown;
        private bool readyToStart;
        private bool startingGame;

        private float timerToStartGame;
        private float notFullGameTimer;
        private float fullGameTimer;

        public float maxWaitTime;
        public float maxFullGameWaitTime;
        
        private void Start()
        {
            myPhotonView = GetComponent<PhotonView>();
            fullGameTimer = maxFullGameWaitTime;
            notFullGameTimer = maxWaitTime;
            timerToStartGame = maxWaitTime;

            PlayerCountUpdate();
        }

        private void PlayerCountUpdate()
        {
            playerCount = PhotonNetwork.PlayerList.Length;
            roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
            roomCountDisplay.text = $"{playerCount}:{roomSize}";

            if (playerCount == roomSize)
            {
                //Room full
                readyToStart = true;
            }
            else if (playerCount >= minPlayersToStart)
            {
                //
                readyToCountDown = true;
            }
            else
            {
                readyToCountDown = false;
                readyToStart = false;
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            PlayerCountUpdate();
            if (PhotonNetwork.IsMasterClient)
            {
                myPhotonView.RPC("RPC_SendTimer", RpcTarget.Others, timerToStartGame);
            }
        }

        [PunRPC]
        private void RPC_SendTimer(float timeIn)
        {
            timerToStartGame = timeIn;
            notFullGameTimer = timeIn;
            if (timeIn < fullGameTimer)
            {
                fullGameTimer = timeIn;
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            PlayerCountUpdate();
        }

        private void Update()
        {
            WaitingForMorePlayers();
        }

        private void WaitingForMorePlayers()
        {
            if (playerCount <= 1)
            {
                ResetTimer();
            }

            if (readyToStart)
            {
                fullGameTimer -= Time.deltaTime;
                timerToStartGame = fullGameTimer;
            }
            else if (readyToCountDown)
            {
                notFullGameTimer -= Time.deltaTime;
                timerToStartGame = notFullGameTimer;
            }

            //format and display countdown timer
            string tempTimer = string.Format("{0:00}", timerToStartGame);
            timerToStartDisplay.text = tempTimer;

            if (timerToStartGame <= 0)
            {
                if (startingGame)
                {
                    return;
                }

                StartGame();
            }
        }

        private void ResetTimer()
        {
            timerToStartGame = maxWaitTime;
            notFullGameTimer = maxWaitTime;
            fullGameTimer = maxFullGameWaitTime;
        }

        public void StartGame()
        {
            startingGame = true;
            if (!PhotonNetwork.IsMasterClient) return;

            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(GlobalValues.gameScene);
        }
        
        
        [PunRPC]
        public void DelayCancel()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                myPhotonView.RPC("DelayCancel",RpcTarget.Others);
            }
            
            
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(GlobalValues.networkScene);
        }
    }
}