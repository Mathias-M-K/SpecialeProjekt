using System;
using Photon.Pun;
using UnityEngine;

namespace DefaultNamespace
{
    public class ChickenGameController : MonoBehaviourPunCallbacks
    {
        private PhotonView _myPhotonView;
        
        private void Start()
        {
            _myPhotonView = GetComponent<PhotonView>();
            _myPhotonView.Group = 2;
        }

        private void Update()
        {
            _myPhotonView.RPC("PrintOut",RpcTarget.Others,"Test message");
        }

        [PunRPC]
        public void PrintOut(string message)
        {
            Debug.LogError($"Not an error: {message}");
        }
    }
}