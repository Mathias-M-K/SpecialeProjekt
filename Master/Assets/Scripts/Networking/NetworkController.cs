using System;
using Photon.Pun;

namespace Networking
{
    public class NetworkController : MonoBehaviourPunCallbacks
    {
        private void Start()
        {
            try
            {
                PhotonNetwork.ConnectUsingSettings();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public override void OnConnectedToMaster()
        {
            print($"Connected to {PhotonNetwork.CloudRegion}");
        }
    }
}