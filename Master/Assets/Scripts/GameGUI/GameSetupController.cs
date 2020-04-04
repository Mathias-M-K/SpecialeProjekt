using System.IO;
using Photon.Pun;
using UnityEngine;

namespace Networking
{
    public class GameSetupController : MonoBehaviourPunCallbacks
    {
        private PhotonView myPhotonView;
        
        private void Start()
        {
            //if (GameHandler.gameHandler.gameType == GameType.Local) return;
            CreatePlayer();
        }

        private void CreatePlayer()
        {
            Debug.Log("Creating Player");
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayer"), new Vector3(1,1,1), Quaternion.identity);
        }
    }
}