using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking.DelayStart
{
    public class DelayStartWaitingRoomController : MonoBehaviourPunCallbacks
    {
        private PhotonView _myPhotonView;

        [SerializeField] private int multiplayerSceneIndex;
        [SerializeField] private int menuSceneIndex;

        [SerializeField]private int _playerCount;
        [SerializeField]private int _roomSize;

        [SerializeField] private int minPlayersToStart;

        [SerializeField] private TextMeshProUGUI roomCountDisplay;
        [SerializeField] private TextMeshProUGUI timerToStartDisplay;

        [SerializeField]private bool _readyToCountDown;
        [SerializeField]private bool _readyToStart;
        [SerializeField]private bool _startingGame;

        [SerializeField]private float _timerToStartGame;
        [SerializeField]private float _notFullGameTimer;
        [SerializeField]private float _fullGameTimer;

        [SerializeField] private float maxWaitTime;
        [SerializeField] private float maxFullGameWaitTime;
        
        // Start is called before the first frame update
        void Start()
        {
            _myPhotonView = GetComponent<PhotonView>();
            _fullGameTimer = maxFullGameWaitTime;
            _notFullGameTimer = maxWaitTime;
            _timerToStartGame = maxWaitTime;

            PlayerCountUpdate();
        }

        private void PlayerCountUpdate()
        {
            _playerCount = PhotonNetwork.PlayerList.Length;
            _roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
            roomCountDisplay.text = _playerCount + ":" + _roomSize;

            if (_playerCount == _roomSize)
            {
                _readyToStart = true;
            }
            else if (_playerCount >= minPlayersToStart)
            {
                _readyToCountDown = true;
            }
            else
            {
                _readyToCountDown = false;
                _readyToStart = false;
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            PlayerCountUpdate();

            if (PhotonNetwork.IsMasterClient)
            {
                _myPhotonView.RPC("RPC_SendTimer",RpcTarget.Others,_timerToStartGame);
            }
        }

        [PunRPC]
        private void RPC_SendTimer(float timeIn)
        {
            _timerToStartGame = timeIn;
            _notFullGameTimer = timeIn;

            if (timeIn < _fullGameTimer)
            {
                _fullGameTimer = timeIn;
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
            if (_playerCount <= 1)
            {
                ResetTimer();
            }

            if (_readyToStart)
            {
                _fullGameTimer -= Time.deltaTime;
                _timerToStartGame = _fullGameTimer;
            }else if (_readyToCountDown)
            {
                _notFullGameTimer -= Time.deltaTime;
                _timerToStartGame = _notFullGameTimer;
            }

            string tempTimer = string.Format("{0:00}", _timerToStartGame);
            timerToStartDisplay.text = tempTimer;

            if (_timerToStartGame <= 0f)
            {
                if (_startingGame) return;
                StartGame();
                
            }
        }

        private void ResetTimer()
        {
            _timerToStartGame = maxWaitTime;
            _notFullGameTimer = maxWaitTime;
            _fullGameTimer = maxFullGameWaitTime;
        }
        
        private void StartGame()
        {
            _startingGame = true;

            if (!PhotonNetwork.IsMasterClient) return;

            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(multiplayerSceneIndex);
        }

        public void Cancel()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(menuSceneIndex);
        }
    }
}
