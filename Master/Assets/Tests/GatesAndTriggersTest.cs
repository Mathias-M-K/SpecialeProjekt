using System.Collections;
using CoreGame;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

namespace Tests
{
    public class GatesAndTriggersTest
    {
        private GameHandler _gameHandler;
        private MapManager _mapManager;

        private GameObject navMesh;

        private GameObject camera;
        private GameObject directionalLight;
        private float waitTime;


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

            waitTime = 1;

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

        [UnityTest]
        public IEnumerator GateShouldOpenRight()
        {
            GateController gc = GameObject.Find("YellowGate").GetComponent<GateController>();
            gc.OpenDirection = Direction.Right;

            Vector3 startPos = gc.transform.position;
            Vector3 assumedEndPos = new Vector3(startPos.x+1,startPos.y,startPos.z);

            gc.Open();
            yield return new WaitForSeconds(waitTime);
            
            Assert.True(gc.transform.position==assumedEndPos);
        }
        
        [UnityTest]
        public IEnumerator GateShouldOpenLeft()
        {
            GateController gc = GameObject.Find("YellowGate").GetComponent<GateController>();
            gc.OpenDirection = Direction.Left;

            Vector3 startPos = gc.transform.position;
            Vector3 assumedEndPos = new Vector3(startPos.x-1,startPos.y,startPos.z);

            gc.Open();
            yield return new WaitForSeconds(waitTime);
            
            Assert.True(gc.transform.position==assumedEndPos);
        }
        
        [UnityTest]
        public IEnumerator GateShouldOpenUp()
        {
            GateController gc = GameObject.Find("RedGate").GetComponent<GateController>();
            gc.OpenDirection = Direction.Up;

            Vector3 startPos = gc.transform.position;
            Vector3 assumedEndPos = new Vector3(startPos.x,startPos.y,startPos.z+1);

            gc.Open();
            yield return new WaitForSeconds(waitTime);
            
            Assert.True(gc.transform.position==assumedEndPos);
        }
        
        [UnityTest]
        public IEnumerator GateShouldOpenDown()
        {
            GateController gc = GameObject.Find("RedGate").GetComponent<GateController>();
            gc.OpenDirection = Direction.Down;

            Vector3 startPos = gc.transform.position;
            Vector3 assumedEndPos = new Vector3(startPos.x,startPos.y,startPos.z-1);

            gc.Open();
            yield return new WaitForSeconds(waitTime);
            
            Assert.True(gc.transform.position==assumedEndPos);
        }
        
        [UnityTest]
        public IEnumerator GateShouldComeBackToOriginalPositionWhenClosing()
        {
            GateController gc = GameObject.Find("RedGate").GetComponent<GateController>();
            gc.OpenDirection = Direction.Down;

            Vector3 startPos = gc.transform.position;
            
            gc.Open();
            yield return new WaitForSeconds(waitTime);
            gc.Close();
            yield return new WaitForSeconds(waitTime);
            
            Assert.True(gc.transform.position==startPos);
        }

        [UnityTest]
        public IEnumerator GateAndTriggerPairShouldHaveSameOwner()
        {
            //The idea is that the trigger sets the o
            GateController gc = GameObject.Find("RedGate").GetComponent<GateController>();
            TriggerController tc = GameObject.Find("RedTrigger").GetComponent<TriggerController>();
            
            Assert.True(gc.Owner == tc.owner);
            yield break;
        }

        [UnityTest]
        public IEnumerator GreenWallShouldMoveIfGreenReachesGreenTrigger()
        {
            PlayerController pc = _gameHandler.GetPlayerController(Player.Green);
            GameObject GreenGate = GameObject.Find("GreenGate");

            Vector3 GreenGateStartPosition = GreenGate.transform.position;
            
            Vector2 greenTriggerPos = new Vector2(10,6);
            
            pc.MoveToPos(greenTriggerPos.x,greenTriggerPos.y);

            while (pc.GetPosition() != greenTriggerPos)
            {
                yield return null;
            }
            
            Assert.That(GreenGate.transform.position != GreenGateStartPosition);
        }
        
        [UnityTest]
        public IEnumerator GreenWallShouldNotMoveIfYellowReachesGreenTrigger()
        {
            PlayerController pc = _gameHandler.GetPlayerController(Player.Yellow);
            GameObject GreenGate = GameObject.Find("GreenGate");

            Vector3 GreenGateStartPosition = GreenGate.transform.position;
            
            Vector2 greenTriggerPos = new Vector2(10,6);
            
            pc.MoveToPos(greenTriggerPos.x,greenTriggerPos.y);

            while (pc.GetPosition() != greenTriggerPos)
            {
                yield return null;
            }
            
            Assert.That(GreenGate.transform.position == GreenGateStartPosition);
        }

        [UnityTest]
        public IEnumerator GreenWallShouldCloseWhenGreenLeavesTriggerAndGateCloseOnTriggerIsEnabled()
        {
            TriggerController tc = GameObject.Find("GreenTrigger").GetComponent<TriggerController>();
            tc.closeOnExit = true;
            
            PlayerController pc = _gameHandler.GetPlayerController(Player.Green);
            GateController GreenGate = GameObject.Find("GreenGate").GetComponent<GateController>();

            Vector3 GreenGateStartPosition = GreenGate.transform.position;
            
            //Setting the easeMethod and animation time to be faster
            GreenGate.easeMethod = LeanTweenType.linear;
            GreenGate.animationTime = 0.3f;
            
            Vector2 greenTriggerPos = new Vector2(10,6);
            
            pc.MoveToPos(greenTriggerPos.x,greenTriggerPos.y);

            while (pc.GetPosition() != greenTriggerPos)
            {
                yield return null;
            }
            yield return new WaitForSeconds(waitTime);
            
            //Making sure the wall actually moved
            Assert.That(GreenGate.transform.position != GreenGateStartPosition);
            
            pc.MovePlayer(Direction.Left);
            
            yield return new WaitForSeconds(waitTime);
            
            //testing if it have moved back
            Assert.That(GreenGate.transform.position == GreenGateStartPosition);
            
            
        }
            
    }
}