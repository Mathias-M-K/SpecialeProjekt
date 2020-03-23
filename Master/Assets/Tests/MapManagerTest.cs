using System.Collections;
using CoreGame;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

namespace Tests
{
    public class MapManagerTest
    {
        private GameHandler _gameHandler;
        private MapManager _mapManager;

        private GameObject navMesh;

        private GameObject camera;
        private GameObject directionalLight;

        private float waitTime = 0.2f;
        
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
            _mapManager.mapData = Resources.Load<MapData>("MapData/4PlayerLevel");
            go.GetComponent<MapManager>().navMeshSurface = navMesh.GetComponent<NavMeshSurface>();
            
            LightmapData lmd = new LightmapData();

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
        
        /*
         * Testing the GenerateMapValues method on 4 player map
         */
        [UnityTest]
        public IEnumerator IndexXOneYOneShouldBePlayerBlue()
        {
            yield return new WaitForSeconds(waitTime);
            string[,] mapValues = _mapManager.GenerateMapValues();
            
            Assert.True(mapValues[1,1].Equals("b"));
        }

        [UnityTest]
        public IEnumerator IndexXFourYSixShouldBeRedGate()
        {
            yield return new WaitForSeconds(waitTime);
            string[,] mapValues = _mapManager.GenerateMapValues();
            
            Assert.True(mapValues[4,6].Equals("rg"));
            
        }

        [UnityTest]
        public IEnumerator IndexXTenYSixShouldBeGreenTrigger()
        {
            yield return new WaitForSeconds(waitTime);
            string[,] mapValues = _mapManager.GenerateMapValues();
            
            Assert.True(mapValues[10,6].Equals("gt"));
        }

        [UnityTest]
        public IEnumerator IndexXFiveYSixShouldBeFinishButton()
        {
            yield return new WaitForSeconds(waitTime);
            string[,] mapValues = _mapManager.GenerateMapValues();
            
            Assert.True(mapValues[5,6].Equals("fp"));
        }
        
        [UnityTest]
        public IEnumerator IndexXThreeYOneShouldBeWall()
        {
            yield return new WaitForSeconds(waitTime);
            string[,] mapValues = _mapManager.GenerateMapValues();
            
            Assert.True(mapValues[3,1].Equals("w"));
        }
        
        [UnityTest]
        public IEnumerator RedShouldBeOnFirstSpawnPosition()
        {
            //Red is always the first one spawned
            Assert.That(_gameHandler.GetPlayerController(Player.Red).GetPosition(), Is.EqualTo(_gameHandler.GetSpawnLocations()[0]));
            yield break;
        }

        [UnityTest]
        public IEnumerator BlueShouldBeOnSecondSpawnPosition()
        {
            //Blue is always the third one spawned
            Assert.That(_gameHandler.GetPlayerController(Player.Blue).GetPosition(), Is.EqualTo(_gameHandler.GetSpawnLocations()[1]));
            yield break;
        }

        [UnityTest]
        public IEnumerator GreenShouldBeOnThirdSpawnPosition()
        {
            //Red is always the first one spawned
            Assert.That(_gameHandler.GetPlayerController(Player.Green).GetPosition(), Is.EqualTo(_gameHandler.GetSpawnLocations()[2]));
            yield break;
        }

        [UnityTest]
        public IEnumerator YellowShouldBeOnForthSpawnPosition()
        {
            //Red is always the first one spawned
            Assert.That(_gameHandler.GetPlayerController(Player.Yellow).GetPosition(), Is.EqualTo(_gameHandler.GetSpawnLocations()[3]));
            yield break;
        }
        
        

    }
}