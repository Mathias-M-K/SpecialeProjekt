using System;
using System.Collections;
using CoreGame;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;


namespace Tests
{
    public class PlayerMovementTest
    {
        private GameObject game;
        private GameHandler _gameHandler;
        private MapManager _mapManager;

        private GameObject navMesh;

        private GameObject camera;
        private GameObject directionalLight;

        private int moveTime;


        [SetUp]
        public void Setup()
        {
            game = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Game"));
            
            _gameHandler = game.GetComponentInChildren<GameHandler>();
            _gameHandler.playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
            _gameHandler.numberOfPlayers = 4;
            
            _mapManager = game.GetComponentInChildren<MapManager>();
            _mapManager.mapData = Resources.Load<MapData>("MapData/4PlayerLevel");
            
            moveTime = 1;

        }

        [TearDown]
        public void Teardown()
        {
            Object.Destroy(game);

            Object.Destroy(GameObject.Find("4 Player Level(Clone)"));
            Object.Destroy(GameObject.Find("Red"));
            Object.Destroy(GameObject.Find("Blue"));
            Object.Destroy(GameObject.Find("Green"));
            Object.Destroy(GameObject.Find("Yellow"));
        }

        
       
        [UnityTest]
        public IEnumerator MoveUpShouldMovePlayerUp()
        {
            _gameHandler.SpawnNewPlayer(Player.Green);
            PlayerController player = _gameHandler.GetPlayerController(Player.Green);
            Vector3 startPos = player.transform.position;
            
            player.MovePlayer(Direction.Up);

            yield return new WaitForSeconds(moveTime);
            
            Assert.That(player.transform.position.x,Is.EqualTo(startPos.x));
            Assert.That(player.transform.position.z,Is.EqualTo(startPos.z+1));
        }
        
        [UnityTest]
        public IEnumerator MoveDownShouldMovePlayerDown()
        {
            _gameHandler.SpawnNewPlayer(Player.Red);
            PlayerController player = _gameHandler.GetPlayerController(Player.Red);
            Vector3 startPos = player.transform.position;
            
            player.MovePlayer(Direction.Down);

            yield return new WaitForSeconds(moveTime);
            
            Assert.That(player.transform.position.x,Is.EqualTo(startPos.x));
            Assert.That(player.transform.position.z,Is.EqualTo(startPos.z-1));
        }
        [UnityTest]
        public IEnumerator MoveRightShouldMovePlayerRight()
        {
            _gameHandler.SpawnNewPlayer(Player.Blue);
            PlayerController player = _gameHandler.GetPlayerController(Player.Blue);
            Vector3 startPos = player.transform.position;
            
            player.MovePlayer(Direction.Right);

            yield return new WaitForSeconds(moveTime);
            
            Assert.That(player.transform.position.x,Is.EqualTo(startPos.x+1));
            Assert.That(player.transform.position.z,Is.EqualTo(startPos.z));
        }

        [UnityTest]
        public IEnumerator MoveLeftShouldMovePlayerLeft()
        {
            _gameHandler.SpawnNewPlayer(Player.Yellow);
            PlayerController player = _gameHandler.GetPlayerController(Player.Yellow);
            Vector3 startPos = player.transform.position;
            
            player.MovePlayer(Direction.Left);

            yield return new WaitForSeconds(moveTime);
            
            Assert.That(player.transform.position.x,Is.EqualTo(startPos.x-1));
            Assert.That(player.transform.position.z,Is.EqualTo(startPos.z));
        }

        [UnityTest]
        public IEnumerator MoveToPosShouldMovePlayerToPos()
        {
            _gameHandler.SpawnNewPlayer(Player.Red);
            Vector3 newPos = new Vector3(1,0,5);
            PlayerController player = _gameHandler.GetPlayerController(Player.Red);
            
            player.MoveToPos(newPos.x,newPos.z);

            while (player.transform.position.z != newPos.z)
            {
                yield return null;
            }
            
            Assert.That(player.transform.position.x,Is.EqualTo(newPos.x));
            Assert.That(player.transform.position.z,Is.EqualTo(newPos.z));
            
        }

        [UnityTest]
        public IEnumerator PlayerShouldNotBeAbleToPhaseThoughWalls()
        {
            _gameHandler.SpawnNewPlayer(Player.Red);
            PlayerController player = _gameHandler.GetPlayerController(Player.Red);
            
            //Moving red so he have a wall to his right
            player.MovePlayer(Direction.Down);
            
            yield return new WaitForSeconds(moveTime);
            
            Assert.Throws<InvalidOperationException>(() => player.MovePlayer(Direction.Right));
        }

        [UnityTest]
        public IEnumerator PlayerShouldNotBeAbleToPhaseThoughGates()
        {
            _gameHandler.SpawnNewPlayer(Player.Red);
            PlayerController player = _gameHandler.GetPlayerController(Player.Red);
            
            Vector2 posThatIsInFronOfGate = new Vector2(3,8);
            player.MoveToPos(3,8);
            
            while (player.transform.position.x == posThatIsInFronOfGate.x)
            {
                yield return null;
            }

            Assert.Throws<InvalidOperationException>(() => player.MovePlayer(Direction.Right));
        }

        [UnityTest]
        public IEnumerator PlayerShouldNotBeAbleToGoOfMap()
        {
            _gameHandler.SpawnNewPlayer(Player.Red);
            PlayerController player = _gameHandler.GetPlayerController(Player.Red);
            
            Assert.Throws<InvalidOperationException>(() => player.MovePlayer(Direction.Left));
            yield break;
        }

        [UnityTest]    //Testing the movePlayer method
        public IEnumerator PlayerShouldLetGameHandlerKnowNewPosition()
        {
            _gameHandler.SpawnNewPlayer(Player.Red);
            PlayerController player = _gameHandler.GetPlayerController(Player.Red);
            Vector2 startPos = player.GetPosition();
            Vector2 endPos = new Vector3(startPos.x,startPos.y-1);
            
            //Asserting that the current position is occupied, and the next is not
            Assert.True(_gameHandler.IsPositionOccupied(startPos));
            Assert.False(_gameHandler.IsPositionOccupied(endPos));
            
            player.MovePlayer(Direction.Down);

            //Asserting that the new position is now occupied, and the old is not
            Assert.False(_gameHandler.IsPositionOccupied(startPos));
            Assert.True(_gameHandler.IsPositionOccupied(endPos));
            
            yield break;
        }
        
        [UnityTest]    //Testing the moveToPos method  
        public IEnumerator PlayerShouldLetGameHandlerKnowNewPositionExtended()
        {
            _gameHandler.SpawnNewPlayer(Player.Red);
            PlayerController player = _gameHandler.GetPlayerController(Player.Red);
            Vector2 startPos = player.GetPosition();
            Vector2 endPos = new Vector2(1,7);
            
            //Asserting that the current position is occupied, and the next is not
            Assert.True(_gameHandler.IsPositionOccupied(startPos));
            Assert.False(_gameHandler.IsPositionOccupied(endPos));
            
            player.MoveToPos(endPos.x,endPos.y);
            
            yield return new WaitForSeconds(0.5f);
            while (player.GetPosition() != endPos)
            {
                yield return null;
            }

            //Asserting that the new position is now occupied, and the old is not
            Assert.False(_gameHandler.IsPositionOccupied(startPos));
            Assert.True(_gameHandler.IsPositionOccupied(endPos));
        }

        [UnityTest]
        public IEnumerator PlayerShouldNotBeAbleToPhaseThoughOtherPlayersIfDisabled()
        {
            _gameHandler.SpawnNewPlayer(Player.Red);
            _gameHandler.SpawnNewPlayer(Player.Blue);
            PlayerController red = _gameHandler.GetPlayerController(Player.Red);
            PlayerController blue = _gameHandler.GetPlayerController(Player.Blue);
            red.MoveToPos(1,6);
            blue.MoveToPos(1,5);
            
            
            while (red.transform.position.z != red.GetComponent<NavMeshAgent>().destination.z || blue.transform.position.z != blue.GetComponent<NavMeshAgent>().destination.z)
            {
                yield return null;
            }
            Assert.Throws<InvalidOperationException>(() => red.MovePlayer(Direction.Down));
        }
        [UnityTest]
        public IEnumerator PlayerShouldBeAbleToPhaseThoughOtherPlayersIfEnabled()
        {
            _gameHandler.SpawnNewPlayer(Player.Red);
            _gameHandler.SpawnNewPlayer(Player.Blue);
            _gameHandler.playersCanPhase = true;
            PlayerController red = _gameHandler.GetPlayerController(Player.Red);
            PlayerController blue = _gameHandler.GetPlayerController(Player.Blue);
            red.MoveToPos(1,6);
            blue.MoveToPos(1,5);
            
            
            while (red.transform.position.z != red.GetComponent<NavMeshAgent>().destination.z || blue.transform.position.z != blue.GetComponent<NavMeshAgent>().destination.z)
            {
                yield return null;
            }
            
            red.MovePlayer(Direction.Down);
            yield return new WaitForSeconds(0.5f);
            
            Assert.True(blue.transform.position == red.transform.position);
        }
    }
}