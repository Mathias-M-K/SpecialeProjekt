﻿using System;
using System.Collections;
using System.Collections.Generic;
using Container;
using UnityEngine;

namespace CoreGame
{
    public class GameHandler : MonoBehaviour
    {
        private readonly List<PlayerMove> _playerMoves = new List<PlayerMove>();
        private readonly List<PlayerController> _players = new List<PlayerController>();
        private readonly List<Vector3> _spawnPositions = new List<Vector3>();
        public List<PlayerTrade> trades = new List<PlayerTrade>();
        private Vector3[] occupiedPositions = new Vector3[4];


        [Header("Player Prefab")] public GameObject player;

        [Space] [Header("Materials")] public Material redMaterial;
        public Material blueMaterial;
        public Material greenMaterial;
        public Material yellowMaterial;

        [Space] [Header("How many players")] [Range(1, 4)]
        public int numberOfPlayers;
        
        [Space] [Header("Player Abilities")]
        public bool playersCollide;

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
            _spawnPositions.Add(new Vector3(1.5f, 2, 10.5f));
            _spawnPositions.Add(new Vector3(1.5f, 2, 1.5f));
            _spawnPositions.Add(new Vector3(10.5f, 2, 1.5f));
            _spawnPositions.Add(new Vector3(10.5f, 2, 10.5f));

            SpawnPlayers();
        }

        public bool IsPositionOccupied(Vector3 position)
        {
            if (playersCollide)
            {
                return false;
            }

            foreach (Vector3 occupiedPosition in occupiedPositions)
            {
                if (position.x == occupiedPosition.x && position.z == occupiedPosition.z)
                {
                    return true;
                }
            }

            return false;
        }

        //Lets players announce their position
        public void RegisterPosition(Player player, Vector3 position)
        {
            switch (player)
            {
                case Player.Red:
                    occupiedPositions[0] = position;
                    break;
                case Player.Blue:
                    occupiedPositions[1] = position;
                    break;
                case Player.Green:
                    occupiedPositions[2] = position;
                    break;
                case Player.Yellow:
                    occupiedPositions[3] = position;
                    break;
                default:
                    throw new ArgumentException("Not a valid player");
            }
        }


        public void OfferMove(Direction d, Player playerReceiving, Player playerOffering)
        {
            if (GetPlayerController(playerOffering).GetDirectionIndex(d) == -1)
            {
                throw new Exception($"{playerOffering} don't own the move that is being offered to {playerReceiving}");
            }

            PlayerController playerOfferingController = GetPlayerController(playerOffering);
            PlayerController playerReceivingController = GetPlayerController(playerReceiving);
            int dIndex =
                playerOfferingController
                    .GetDirectionIndex(d); //The index at which the move is stored at the playercontroller

            PlayerTrade trade = new PlayerTrade(playerOffering, playerReceiving, d, this, dIndex);

            trades.Add(trade);
            playerReceivingController.QueTrade(trade);

            playerOfferingController.RemoveMove(dIndex);
        }


        public void AddMoveToSequece(Player p, Direction d)
        {
            if (GetPlayerController(p) == null)
            {
                Debug.LogException(new ArgumentException(p + " is not active"), this);
                return;
            }

            PlayerMove playerMove = new PlayerMove(p, d);
            _playerMoves.Add(playerMove);
        }

        public IEnumerator PerformSequence(float delayBetweenMoves)
        {
            foreach (PlayerMove pm in _playerMoves)
            {
                PlayerController playerController = GetPlayerController(pm.Player);

                playerController.MovePlayer(pm.Direction);
                yield return new WaitForSeconds(delayBetweenMoves);
            }
        }

        public PlayerController GetPlayerController(Player p)
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

        public List<Vector3> GetSpawnLocations()
        {
            return _spawnPositions;
        }

        private void SpawnPlayers()
        {
            if (numberOfPlayers > 4)
            {
                Debug.LogError("Number of max players have been exceeded, fallback to 4 players", this);
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

                Material m;
                Player playerColor;

                switch (playerColors[i])
                {
                    case Player.Red:
                        m = redMaterial;
                        break;
                    case Player.Blue:
                        m = blueMaterial;
                        break;
                    case Player.Green:
                        m = greenMaterial;
                        break;
                    case Player.Yellow:
                        m = yellowMaterial;
                        break;
                    default:
                        m = redMaterial;
                        break;
                }

                occupiedPositions[i] = _spawnPositions[i];
                PlayerController p = g.GetComponent<PlayerController>();

                p.SetCamera(Camera.main);
                p.SetGameHandler(this);
                p.SetColor(m);
                _players.Add(p);
            }
        }

        public List<PlayerController> GetPlayers()
        {
            return _players;
        }
    }
}