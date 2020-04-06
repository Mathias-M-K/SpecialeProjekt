using System;
using CoreGame;
using Photon.Pun;

namespace DefaultNamespace
{
    public class NetworkedGameHandler : GameHandler
    {
        public GameHandler localGameHandler;

        public override void Awake()
        {
            localGameHandler = gameObject.AddComponent<GameHandler>();
            GameHandler.Current = this;
        }
        private PhotonView _photonView;
        
        private void Start()
        {
            gameObject.AddComponent<GameHandler>();
            localGameHandler.playerPrefab = playerPrefab;
            localGameHandler.numberOfPlayers = numberOfPlayers;
            localGameHandler.delayBetweenMoves = delayBetweenMoves;
            localGameHandler.playersAreExternallyControlled = playersAreExternallyControlled;
            localGameHandler.playersCanPhase = playersCanPhase;
            localGameHandler.endScreen = endScreen;
        }

        /**
         * Setup
         */
        public override void SetMapData(MapData mapData)
        {
            localGameHandler.SetMapData(mapData);
        }
        
        public void SetPhotonView(PhotonView photonView)
        {
            _photonView = photonView;
        }

        public override void SetNetworkedAgent(NetworkAgentController netController)
        {
            MyNetworkedAgent = netController;
            netController.SetGameHandler(localGameHandler);
        }
        
        
        /**
         * Network Methods
         */

        public override PlayerTags SpawnNewPlayer()
        {
            print("Lol");
            MyNetworkedAgent.SpawnNewPlayer();
            return localGameHandler.SpawnNewPlayer();
        }

        
    }
}