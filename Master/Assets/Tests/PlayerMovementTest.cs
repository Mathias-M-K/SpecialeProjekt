using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using CoreGame;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEditor.PackageManager;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Tests
{
    public class PlayerMovementTest
    {
        private GameHandler _gameHandler;
        private MapManager _mapManager;

        private GameObject navMesh;

        private GameObject camera;
        private GameObject directionalLight;


        [SetUp]
        public void Setup()
        {
            camera = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/MainCamera"));
            directionalLight = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/DirectionalLight"));
            GameObject go = new GameObject();
            go.AddComponent<GameHandler>();
            go.AddComponent<MapManager>();

            _gameHandler = go.GetComponent<GameHandler>();
            _gameHandler.playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
            _gameHandler.numberOfPlayers = 4;

            navMesh = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/NavMesh"));
            MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/ColorPalette"));

            _mapManager = go.GetComponent<MapManager>();
            _mapManager.mapData = Resources.Load<MapData>("Maps/4PlayerLevel");
            go.GetComponent<MapManager>().navMeshSurface = navMesh.GetComponent<NavMeshSurface>();
        }

        [TearDown]
        public void Teardown()
        {
            Object.Destroy(camera);
            Object.Destroy(directionalLight);
            Object.Destroy(_gameHandler.gameObject);
            Object.Destroy(navMesh);

            Object.Destroy(ColorPalette.current.gameObject);


            Object.Destroy(GameObject.Find("4 Player Level(Clone)"));
            Object.Destroy(GameObject.Find("Red"));
            Object.Destroy(GameObject.Find("Blue"));
            Object.Destroy(GameObject.Find("Green"));
            Object.Destroy(GameObject.Find("Yellow"));
        }

        // A Test behaves as an ordinary method
        [UnityTest]
        public IEnumerator TestSpawnPositionOne()
        {
            Assert.That(_gameHandler.GetPlayerController(Player.Red).gameObject.transform.position.x, Is.EqualTo(1));
            Assert.That(_gameHandler.GetPlayerController(Player.Red).gameObject.transform.position.z, Is.EqualTo(10));
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSpawnPositionTwo()
        {
            Assert.That(_gameHandler.GetPlayerController(Player.Blue).gameObject.transform.position.x, Is.EqualTo(1));
            Assert.That(_gameHandler.GetPlayerController(Player.Blue).gameObject.transform.position.z, Is.EqualTo(1));
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSpawnPositionThree()
        {
            Assert.That(_gameHandler.GetPlayerController(Player.Green).gameObject.transform.position.x, Is.EqualTo(10));
            Assert.That(_gameHandler.GetPlayerController(Player.Green).gameObject.transform.position.z, Is.EqualTo(1));
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSpawnPositionFour()
        {
            Assert.That(_gameHandler.GetPlayerController(Player.Yellow).gameObject.transform.position.x, Is.EqualTo(10));
            Assert.That(_gameHandler.GetPlayerController(Player.Yellow).gameObject.transform.position.z, Is.EqualTo(10));
            yield break;
        }
        [UnityTest]
        public IEnumerator MoveUpShouldMovePlayerUp()
        {
            PlayerController player = _gameHandler.GetPlayerController(Player.Green);
            Vector3 startPos = player.transform.position;
            
            player.MovePlayer(Direction.Up);

            yield return new WaitForSeconds(1);
            
            Assert.That(player.transform.position.x,Is.EqualTo(startPos.x));
            Assert.That(player.transform.position.z,Is.EqualTo(startPos.z+1));
        }
        
        [UnityTest]
        public IEnumerator MoveDownShouldMovePlayerDown()
        {
            PlayerController player = _gameHandler.GetPlayerController(Player.Red);
            Vector3 startPos = player.transform.position;
            
            player.MovePlayer(Direction.Down);

            yield return new WaitForSeconds(1);
            
            Assert.That(player.transform.position.x,Is.EqualTo(startPos.x));
            Assert.That(player.transform.position.z,Is.EqualTo(startPos.z-1));
        }
        [UnityTest]
        public IEnumerator MoveRightShouldMovePlayerRight()
        {
            PlayerController player = _gameHandler.GetPlayerController(Player.Blue);
            Vector3 startPos = player.transform.position;
            
            player.MovePlayer(Direction.Right);

            yield return new WaitForSeconds(1);
            
            Assert.That(player.transform.position.x,Is.EqualTo(startPos.x+1));
            Assert.That(player.transform.position.z,Is.EqualTo(startPos.z));
        }

        [UnityTest]
        public IEnumerator MoveLeftShouldMovePlayerLeft()
        {
            PlayerController player = _gameHandler.GetPlayerController(Player.Yellow);
            Vector3 startPos = player.transform.position;
            
            player.MovePlayer(Direction.Left);

            yield return new WaitForSeconds(1);
            
            Assert.That(player.transform.position.x,Is.EqualTo(startPos.x-1));
            Assert.That(player.transform.position.z,Is.EqualTo(startPos.z));
        }

        [UnityTest]
        public IEnumerator MoveToPosShouldMovePlayerToPos()
        {
            Vector3 newPos = new Vector3(8,0,3);
            PlayerController player = _gameHandler.GetPlayerController(Player.Red);
            
            player.MoveToPos(newPos.x,newPos.z);

            yield return new WaitForSeconds(7);
            
            Assert.That(player.transform.position.x,Is.EqualTo(newPos.x));
            Assert.That(player.transform.position.z,Is.EqualTo(newPos.z));
            
        }

        [UnityTest]
        public IEnumerator PlayerShouldNotBeAbleToPhaseThoughWalls()
        {
            PlayerController player = _gameHandler.GetPlayerController(Player.Red);
            
            //Moving red so he have a wall to his right
            player.MovePlayer(Direction.Down);
            
            yield return new WaitForSeconds(0.5f);
            
            Assert.Throws<ArgumentException>(() => player.MovePlayer(Direction.Right));
        }

        [UnityTest]
        public IEnumerator PlayerShouldNotBeAbleToGoOfMap()
        {
            PlayerController player = _gameHandler.GetPlayerController(Player.Red);
            
            Assert.Throws<ArgumentException>(() => player.MovePlayer(Direction.Left));
            yield break;
        }

        [UnityTest]    //Testing the movePlayer method
        public IEnumerator PlayerShouldLetGameHandlerKnowNewPosition()
        {
            PlayerController player = _gameHandler.GetPlayerController(Player.Red);
            Vector3 startPos = player.transform.position;
            Vector3 endPos = new Vector3(startPos.x,startPos.y,startPos.z-1);
            
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
            PlayerController player = _gameHandler.GetPlayerController(Player.Red);
            Vector3 startPos = player.transform.position;
            Vector3 endPos = new Vector3(1,startPos.y,7);
            
            //Asserting that the current position is occupied, and the next is not
            Assert.True(_gameHandler.IsPositionOccupied(startPos));
            Assert.False(_gameHandler.IsPositionOccupied(endPos));
            
            player.MoveToPos(endPos.x,endPos.z);
            
            yield return new WaitForSeconds(0.5f);
            while (player.transform.position.z != endPos.z)
            {
                yield return null;
            }

            //Asserting that the new position is now occupied, and the old is not
            Assert.False(_gameHandler.IsPositionOccupied(startPos));
            Assert.True(_gameHandler.IsPositionOccupied(endPos));
            
            yield break;
        }

        [UnityTest]
        public IEnumerator PlayerShouldNotBeAbleToPhaseThoughOtherPlayersIfDisabled()
        {
            PlayerController red = _gameHandler.GetPlayerController(Player.Red);
            PlayerController blue = _gameHandler.GetPlayerController(Player.Blue);
            red.MoveToPos(1,6);
            blue.MoveToPos(1,5);
            
            
            while (red.transform.position.z != red.GetComponent<NavMeshAgent>().destination.z || blue.transform.position.z != blue.GetComponent<NavMeshAgent>().destination.z)
            {
                yield return null;
            }
            Assert.Throws<ArgumentException>(() => red.MovePlayer(Direction.Down));
        }
        [UnityTest]
        public IEnumerator PlayerShouldBeAbleToPhaseThoughOtherPlayersIfEnabled()
        {
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