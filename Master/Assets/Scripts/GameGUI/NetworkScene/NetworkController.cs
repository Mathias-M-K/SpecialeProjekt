using System;
using Photon.Pun;

namespace GameGUI.NetworkScene
{
    public class NetworkController : MonoBehaviourPunCallbacks
    {
        private void Start()
        {
            if (!PhotonNetwork.IsConnected) PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            print($"Connected to {PhotonNetwork.CloudRegion}");
        }
    }
}