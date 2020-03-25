using System;
using System.Collections;
using CoreGame;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Tests
{
    public class GameHandlerTest
    {
        private GameObject game;
        private GameHandler _gameHandler;
        private MapManager _mapManager;

        private GameObject navMesh;

        private GameObject camera;
        private GameObject directionalLight;


        [SetUp]
        public void Setup()
        {
            game = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Game"));


            _gameHandler = game.GetComponentInChildren<GameHandler>();
            _gameHandler.playersAreExternallyControlled = true;
            _gameHandler.playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
            _gameHandler.numberOfPlayers = 4;

            _mapManager = game.GetComponentInChildren<MapManager>();
            _mapManager.mapData = Resources.Load<MapData>("MapData/4PlayerLevel");
            
            _gameHandler.StartGame();
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
        public IEnumerator FirstSpawnShouldBeRed()
        {
            Player p = _gameHandler.SpawnNewPlayer();
            Assert.True(p == Player.Red);

            yield break;
        }
        
        [UnityTest]
        public IEnumerator SecondSpawnShouldBeBlue()
        {
            _gameHandler.SpawnNewPlayer();
            Assert.True(_gameHandler.SpawnNewPlayer() == Player.Blue);
            yield break;
        }
        
        [UnityTest]
        public IEnumerator ThirdSpawnShouldBeGreen()
        {
            for (int i = 0; i < 2; i++)
            {
                _gameHandler.SpawnNewPlayer();
            }
            Assert.True(_gameHandler.SpawnNewPlayer() == Player.Green);
            yield break;
        }
        
        [UnityTest]
        public IEnumerator ForthSpawnShouldBeYellow()
        {
            for (int i = 0; i < 3; i++)
            {
                _gameHandler.SpawnNewPlayer();
            }
            Assert.True(_gameHandler.SpawnNewPlayer() == Player.Yellow);
            yield break;
        }

        [UnityTest]
        public IEnumerator GameFinishedIfFourOfFourPlayersAreDone()
        {
            GameObject.Find("RedGate").GetComponent<GateController>().Open();
            GameObject.Find("BlueGate").GetComponent<GateController>().Open();
            GameObject.Find("GreenGate").GetComponent<GateController>().Open();
            GameObject.Find("YellowGate").GetComponent<GateController>().Open();
            
            _gameHandler.SpawnNewPlayer();
            _gameHandler.SpawnNewPlayer();
            _gameHandler.SpawnNewPlayer();
            _gameHandler.SpawnNewPlayer();
            yield return new WaitForSeconds(0.3f);
            
            PlayerController red = _gameHandler.GetPlayerController(Player.Red);
            PlayerController blue = _gameHandler.GetPlayerController(Player.Blue);
            PlayerController green = _gameHandler.GetPlayerController(Player.Green);
            PlayerController yellow = _gameHandler.GetPlayerController(Player.Yellow);
            
            Vector2 finishPoint = new Vector2(5,6);
            
            //Making sure game is not done at the moment
            Assert.False(_gameHandler.IsGameDone);
            
            blue.MoveToPos(finishPoint);
            green.MoveToPos(finishPoint);
            yellow.MoveToPos(finishPoint);
            
            //Delaying red so he will be the last!
            yield return new WaitForSeconds(1);
            red.MoveToPos(finishPoint);
            
            while (red != null && red.GetPosition() != finishPoint)
            {
                yield return null;
            }
            
            Assert.True(_gameHandler.IsGameDone);
        }
        
        [UnityTest]
        public IEnumerator GameFinishedIfThreeOfThreePlayersAreDone()
        {
            GameObject.Find("RedGate").GetComponent<GateController>().Open();
            GameObject.Find("BlueGate").GetComponent<GateController>().Open();
            GameObject.Find("GreenGate").GetComponent<GateController>().Open();

            _gameHandler.SpawnNewPlayer();
            _gameHandler.SpawnNewPlayer();
            _gameHandler.SpawnNewPlayer();

            yield return new WaitForSeconds(0.3f);
            
            PlayerController red = _gameHandler.GetPlayerController(Player.Red);
            PlayerController blue = _gameHandler.GetPlayerController(Player.Blue);
            PlayerController green = _gameHandler.GetPlayerController(Player.Green);

            Vector2 finishPoint = new Vector2(5,6);
            
            blue.MoveToPos(finishPoint);
            green.MoveToPos(finishPoint);
            
            yield return new WaitForSeconds(1);
            red.MoveToPos(finishPoint);
            
            while (red != null && red.GetPosition() != finishPoint)
            {
                yield return null;
            }
            
            Assert.True(_gameHandler.IsGameDone);
        }
        
        [UnityTest]
        public IEnumerator GameFinishedIfTwoOfTwoPlayersAreDone()
        {
            GameObject.Find("RedGate").GetComponent<GateController>().Open();
            GameObject.Find("BlueGate").GetComponent<GateController>().Open();

            _gameHandler.SpawnNewPlayer();
            _gameHandler.SpawnNewPlayer();

            yield return new WaitForSeconds(0.3f);
            
            PlayerController red = _gameHandler.GetPlayerController(Player.Red);
            PlayerController blue = _gameHandler.GetPlayerController(Player.Blue);

            Vector2 finishPoint = new Vector2(5,6);
            
            blue.MoveToPos(finishPoint);

            yield return new WaitForSeconds(1);
            red.MoveToPos(finishPoint);
            
            while (red != null && red.GetPosition() != finishPoint)
            {
                yield return null;
            }
            
            Assert.True(_gameHandler.IsGameDone);
        }

        [UnityTest]
        public IEnumerator GameNotFinishedIfThereArePlayersLeft()
        {
            GameObject.Find("RedGate").GetComponent<GateController>().Open();
            GameObject.Find("BlueGate").GetComponent<GateController>().Open();
            GameObject.Find("GreenGate").GetComponent<GateController>().Open();
            GameObject.Find("YellowGate").GetComponent<GateController>().Open();
            
            _gameHandler.SpawnNewPlayer();
            _gameHandler.SpawnNewPlayer();
            _gameHandler.SpawnNewPlayer();
            _gameHandler.SpawnNewPlayer();
            yield return new WaitForSeconds(0.3f);

            PlayerController blue = _gameHandler.GetPlayerController(Player.Blue);
            PlayerController green = _gameHandler.GetPlayerController(Player.Green);
            PlayerController yellow = _gameHandler.GetPlayerController(Player.Yellow);
            
            Vector2 finishPoint = new Vector2(5,6);
            
            //Making sure game is not done at the moment
            Assert.False(_gameHandler.IsGameDone);
            
            blue.MoveToPos(finishPoint);
            green.MoveToPos(finishPoint);
            
            yield return new WaitForSeconds(1);
            yellow.MoveToPos(finishPoint);
            
            
            while (yellow != null && yellow.GetPosition() != finishPoint)
            {
                yield return null;
            }
            
            Assert.False(_gameHandler.IsGameDone);
        }
        
        [UnityTest]
        public IEnumerator PlayerShouldLooseMoveWhenAddingItToSequence()
        {
            _gameHandler.SpawnNewPlayer();
            PlayerController playerController = _gameHandler.GetPlayerController(Player.Red);

            //Where in the direction array the move is stored
            int moveStoreIndex = playerController.GetIndexForDirection(Direction.Up);
            
            //adding move, which in turn, should remove it from the player so he is not able to use it multiple times
            _gameHandler.AddMoveToSequence(Player.Red,Direction.Up,moveStoreIndex);
            
            //The player should now have a blank playerMove instead
            Assert.True(playerController.GetMoves()[moveStoreIndex] == Direction.Blank);
            yield break;
        }

        [UnityTest]
        public IEnumerator PlayerShouldNotBeAbleToUseMovesNotInPossession()
        {
            _gameHandler.SpawnNewPlayer();
            PlayerController playerController = _gameHandler.GetPlayerController(Player.Red);
            
            //Adding Move to sequence. After this player should not be able to add the same move again
            _gameHandler.AddMoveToSequence(Player.Red,Direction.Up,playerController.GetIndexForDirection(Direction.Up));

            Assert.Throws<InvalidOperationException>(() => _gameHandler.AddMoveToSequence(
                Player.Red, 
                Direction.Up, 
                playerController.GetIndexForDirection(Direction.Up))
            );
            
            yield break;
        }

        [UnityTest]
        public IEnumerator PlayerShouldBeAbleToUseTheSameMoveMultipleTimesIfInPossession()
        {
            _gameHandler.SpawnNewPlayer();
            PlayerController playerController = _gameHandler.GetPlayerController(Player.Red);
            
            //Making so, that all player moves are down
            playerController.AddMove(Direction.Down,0);
            playerController.AddMove(Direction.Down,1);
            playerController.AddMove(Direction.Down,2);
            playerController.AddMove(Direction.Down,3);
            
            //Player should now be able to move down four times
            Assert.DoesNotThrow(() => _gameHandler.AddMoveToSequence(Player.Red,Direction.Down,playerController.GetIndexForDirection(Direction.Down)));
            Assert.DoesNotThrow(() => _gameHandler.AddMoveToSequence(Player.Red,Direction.Down,playerController.GetIndexForDirection(Direction.Down)));
            Assert.DoesNotThrow(() => _gameHandler.AddMoveToSequence(Player.Red,Direction.Down,playerController.GetIndexForDirection(Direction.Down)));
            Assert.DoesNotThrow(() => _gameHandler.AddMoveToSequence(Player.Red,Direction.Down,playerController.GetIndexForDirection(Direction.Down)));
            
            yield break;
        }

        [UnityTest]
        public IEnumerator InactivePlayersCannotAddMoves()
        {
            _gameHandler.SpawnNewPlayer();
            PlayerController playerController = _gameHandler.GetPlayerController(Player.Red);
            
            Assert.DoesNotThrow(()=> _gameHandler.AddMoveToSequence(Player.Red, Direction.Up, playerController.GetIndexForDirection(Direction.Up)));
            Assert.Throws<ArgumentException>(()=> _gameHandler.AddMoveToSequence(Player.Blue, Direction.Up, 2));    //index doesn't matter, since it should be stopped beforehand
            
            yield break;
        }

        [UnityTest]
        public IEnumerator GetPlayerControllerShouldReturnTheCorrectPlayerController()
        {
            _gameHandler.SpawnMaxPlayers();
            
            Assert.That(_gameHandler.GetPlayerController(Player.Red).player == Player.Red);
            Assert.That(_gameHandler.GetPlayerController(Player.Blue).player == Player.Blue);
            Assert.That(_gameHandler.GetPlayerController(Player.Green).player == Player.Green);
            Assert.That(_gameHandler.GetPlayerController(Player.Yellow).player == Player.Yellow);
            
            yield break;
        }

        [UnityTest]
        public IEnumerator SequencePlaysTwoOfTwoPlayersAreReady()
        {
            _gameHandler.SpawnNewPlayer();
            _gameHandler.SpawnNewPlayer();

            PlayerController red = _gameHandler.GetPlayerController(Player.Red);
            PlayerController blue = _gameHandler.GetPlayerController(Player.Blue);

            Vector2 redStartPos = red.GetPosition();

            //Red adds down to sequence. If sequence is played we know that red will down one position
            _gameHandler.AddMoveToSequence(Player.Red,Direction.Down,red.GetIndexForDirection(Direction.Down));
            Assert.True(redStartPos == red.GetPosition());

            //sets all players to ready
            red.Ready = true;
            blue.Ready = true;
            
            //Waiting the time it takes to perform a move
            yield return new WaitForSeconds(0.5f);

            //Red should now be on the position below its original position
            Assert.True(red.GetPosition() == new Vector2(redStartPos.x,redStartPos.y-1));
        }

        [UnityTest]
        public IEnumerator SequenceDoesNotPlayIfOneOfTwoPlayersAreReady()
        {
            _gameHandler.SpawnNewPlayer();
            _gameHandler.SpawnNewPlayer();

            PlayerController red = _gameHandler.GetPlayerController(Player.Red);

            Vector2 redStartPos = red.GetPosition();

            //Red adds down to sequence. If sequence is played we know that red will down one position
            _gameHandler.AddMoveToSequence(Player.Red,Direction.Down,red.GetIndexForDirection(Direction.Down));
            Assert.True(redStartPos == red.GetPosition());

            //Set red to ready, but not blue!
            red.Ready = true;

            //Waiting the time it takes to perform a move
            yield return new WaitForSeconds(0.5f);

            //Red should now be on the exact same position as before
            Assert.True(red.GetPosition() == new Vector2(redStartPos.x,redStartPos.y));
            
            
        }

        [UnityTest]
        public IEnumerator SpawnMaxPlayersShouldSpawnPlayersUntilThereIsNotAnyMoreSpawnPoints()
        {
            _gameHandler.SpawnMaxPlayers();
            
            Assert.True(_gameHandler.GetPlayerController(Player.Red) != null);
            Assert.True(_gameHandler.GetPlayerController(Player.Blue) != null);
            Assert.True(_gameHandler.GetPlayerController(Player.Green) != null);
            Assert.True(_gameHandler.GetPlayerController(Player.Yellow) != null);
            
            yield break;
        }

        [UnityTest]
        public IEnumerator SpawnMaxPlayersShouldSpawnPlayersOnTheSpawnPositionsLeftEvenIfOnePlayerIsAlreadySpawned()
        {
            _gameHandler.SpawnNewPlayer();
            
            Assert.True(_gameHandler.GetPlayerController(Player.Red) != null);
            Assert.True(_gameHandler.GetPlayerController(Player.Blue) == null);
            Assert.True(_gameHandler.GetPlayerController(Player.Green) == null);
            Assert.True(_gameHandler.GetPlayerController(Player.Yellow) == null);

            _gameHandler.SpawnMaxPlayers();
            
            Assert.True(_gameHandler.GetPlayerController(Player.Red) != null);
            Assert.True(_gameHandler.GetPlayerController(Player.Blue) != null);
            Assert.True(_gameHandler.GetPlayerController(Player.Green) != null);
            Assert.True(_gameHandler.GetPlayerController(Player.Yellow) != null);
            
            yield break;
        }

        [UnityTest]
        public IEnumerator SpawningSpecificPlayerShouldSpawnThatPlayerAndNoOther()
        {
            _gameHandler.SpawnNewPlayer(Player.Green);
            yield return new WaitForSeconds(2);
            
            //player that should be spawned
            Assert.True(_gameHandler.GetPlayerController(Player.Green) != null);
            
            //players that should not be spawned
            Assert.True(_gameHandler.GetPlayerController(Player.Red) == null);
            Assert.True(_gameHandler.GetPlayerController(Player.Blue) == null);
            Assert.True(_gameHandler.GetPlayerController(Player.Yellow) == null);
            
        }

        [UnityTest]
        public IEnumerator SpawnNewPlayerShouldSpawnNextPlayerInLine()
        {
            _gameHandler.SpawnNewPlayer(Player.Green);
            
            //the next player to be spawned should be red, so just making sure he is not there already
            Assert.True(_gameHandler.GetPlayerController(Player.Green) != null);
            Assert.True(_gameHandler.GetPlayerController(Player.Red) == null);

            _gameHandler.SpawnNewPlayer();
            Assert.True(_gameHandler.GetPlayerController(Player.Red) != null);
            
            yield break;
        }

        [UnityTest]
        public IEnumerator PlayerCanNotBeSpawnedMoreThanOneTime()
        {
            _gameHandler.SpawnNewPlayer(Player.Green);

            Assert.Throws<ArgumentException>(() => _gameHandler.SpawnNewPlayer(Player.Green));
            yield break;
        }

        [UnityTest]
        public IEnumerator PlayerBlueIsNotNextInLineIfAlreadySpawned()
        {
            _gameHandler.SpawnNewPlayer(Player.Blue);
            Assert.True(_gameHandler.GetPlayerController(Player.Blue) != null);
            Assert.True(_gameHandler.GetPlayerController(Player.Red) == null);

            //this should spawn red
            _gameHandler.SpawnNewPlayer();
            Assert.True(_gameHandler.GetPlayerController(Player.Red) != null);
            Assert.True(_gameHandler.GetPlayerController(Player.Green) == null);

            //Should spawn green
            _gameHandler.SpawnNewPlayer();
            Assert.True(_gameHandler.GetPlayerController(Player.Green) != null);
            Assert.True(_gameHandler.GetPlayerController(Player.Yellow) == null);
            
            //Should spawn yellow
            _gameHandler.SpawnNewPlayer();
            Assert.True(_gameHandler.GetPlayerController(Player.Yellow) != null);
            
            yield break;
        }
        
        
    }
}