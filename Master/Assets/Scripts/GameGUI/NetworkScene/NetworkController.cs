using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace GameGUI.NetworkScene
{
    public class NetworkController : MonoBehaviourPunCallbacks
    {
        public TextMeshProUGUI regionText;
        public TextMeshProUGUI appVersionText;
        public TextMeshProUGUI serverText;


        private void Start()
        {
            if(PhotonNetwork.IsConnected) SetText();
            if (!PhotonNetwork.IsConnected) PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            SetText();
        }

        private void SetText()
        {
            regionText.text = $"Region : {PhotonNetwork.CloudRegion}";
            appVersionText.text = $"AppVersion : {PhotonNetwork.AppVersion}";
            serverText.text = $"Server : {PhotonNetwork.Server}";
        }
    }
}