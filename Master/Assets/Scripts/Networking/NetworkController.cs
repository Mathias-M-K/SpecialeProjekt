using System;
using Photon.Pun;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log($"We are now connected to the {PhotonNetwork.CloudRegion} server");
    }
}
