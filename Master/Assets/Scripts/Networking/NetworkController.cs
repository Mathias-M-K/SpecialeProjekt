using Photon.Pun;

namespace Networking
{
    public class NetworkController : MonoBehaviourPunCallbacks
    {
        private void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            print($"Connected to {PhotonNetwork.CloudRegion}");
        }
    }
}