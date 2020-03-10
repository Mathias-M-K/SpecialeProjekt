using System;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace CoreGame
{
    public class LevelInformation : MonoBehaviour
    {
        [Space] [Header("Map Info")] 
        public int xSize;
        public int ySize;
        
        [Space] [Header("Game Info")] 
        public int maxPlayers;
        public Vector2[] spawnPositions;

        private void Start()
        {
            Camera camera = Camera.main;

            if (camera != null) camera.transform.position = new Vector3((xSize / 2) + 0.5f, 15, (ySize / 2) + 0.5f);
        }
    }
}
