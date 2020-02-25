using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public class GameHandler : MonoBehaviour
    {
        private readonly List<PlayerMove> _playerMoves = new List<PlayerMove>();
        private readonly List<PlayerController> _players = new List<PlayerController>();
        private readonly List<Vector3> _spawnPositions = new List<Vector3>();
        
        [Header("Player Prefab")]
        public GameObject player;
        [Space]
        
        [Header("How many players")][Range(0,4)]
        public int numberOfPlayers;

        private struct PlayerMove
        {
            public readonly Direction Direction;
            public readonly Player Player;

            public PlayerMove(Player p, Direction d)
            {
                Player = p;
                Direction = d;
            }
        }

        private void Awake()
        {
            _spawnPositions.Add(new Vector3(1.5f,2,10.5f));
            _spawnPositions.Add(new Vector3(1.5f,2,1.5f));
            _spawnPositions.Add(new Vector3(10.5f,2,1.5f));
            _spawnPositions.Add(new Vector3(10.5f,2,10.5f));
            
            SpawnPlayers();
        }

        public void AddMove(Player p, Direction d)
        {
            if (GetPlayerController(p) == null)
            {
                Debug.LogException(new ArgumentException(p + " is not active"),this);
                return;
            }
            
            PlayerMove playerMove = new PlayerMove(p,d);
            _playerMoves.Add(playerMove);
        }

        public IEnumerator PerformMoves(float delayBetweenMoves)
        {
            foreach(PlayerMove pm in _playerMoves)
            {
                PlayerController playerController = GetPlayerController(pm.Player);
                
                playerController.MovePlayer(pm.Direction);
                yield return new WaitForSeconds(delayBetweenMoves);
            }
        }

        private void SpawnPlayers()
        {
            if (numberOfPlayers > 4)
            {
                Debug.LogError("Number of max players have been exceeded, fallback to 4 players",this);
                numberOfPlayers = 4;
            }
            
            List<Player> playerColors = new List<Player>();
            playerColors.Add(Player.Red);
            playerColors.Add(Player.Blue);
            playerColors.Add(Player.Green);
            playerColors.Add(Player.Yellow);

            for (int i = 0; i < numberOfPlayers; i++)
            {
                GameObject g = Instantiate(player, _spawnPositions[i], new Quaternion(0, 0, 0, 0));
                PlayerController p = g.GetComponent<PlayerController>();

                p.player = playerColors[i];
                p.cam = Camera.main;
                p.SetColor();
                _players.Add(p);
            }
        }

        private PlayerController GetPlayerController(Player p)
        {
            foreach (var playerController in _players)
            {
                if (playerController.player == p)
                {
                    return playerController;
                }
            }

            return null;
        }


    }
}

