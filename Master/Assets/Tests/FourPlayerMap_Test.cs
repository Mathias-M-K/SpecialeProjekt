using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Boo.Lang.Environments;
using CoreGame;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class FourPlayerMap_Test
    {
        private MapData _mapData = Resources.Load<MapData>("Maps/4PlayerLevel");
        private GameObject go = new GameObject("GameHandler");
        
        private GameHandler _gameHandler;
        private MapManager _mapManager;
        
        [SetUp]
        public void Setup()
        {
            go.AddComponent<GameHandler>();
            go.AddComponent<MapManager>();
            
            _gameHandler = go.GetComponent<GameHandler>();
            _mapManager = go.GetComponent<MapManager>();

            _mapManager.mapData = _mapData;
        }

        [UnityTest]
        public IEnumerator whatever()
        {
            _gameHandler.delayBetweenMoves = 2;
            
            Assert.That(_gameHandler.delayBetweenMoves, Is.EqualTo(2));
            
            yield return new WaitForSeconds(2);
        }
        
    }
}
