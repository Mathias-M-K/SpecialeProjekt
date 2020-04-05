using System;
using Photon.Pun;
using UnityEngine;

namespace DefaultNamespace
{
    public class ChickenGameController : MonoBehaviourPunCallbacks
    {
        public static ChickenGameController Current;
        
        private void Awake()
        {
            Current = this;
        }

        public ElevatorController[] elevators;
        private PhotonView _myPhotonView;
        
        private void Start()
        {
            _myPhotonView = GetComponent<PhotonView>();
            _myPhotonView.Group = 2;
            PhotonNetwork.SetInterestGroups(2,true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                _myPhotonView.RPC("PrintOut",RpcTarget.Others,"Test message");
            }
        }
    }
}