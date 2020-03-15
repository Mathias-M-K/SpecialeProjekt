using System;
using UnityEngine;
using UnityEngine.AI;

// ReSharper disable PossibleLossOfFraction
// ReSharper disable once PossibleNullReferenceException
// ReSharper disable once Unity.PerformanceCriticalCodeInvocation

namespace CoreGame
{
    public class MapManager : MonoBehaviour
    {
        
        [Header("Level Information")] public MapData mapData;
        public NavMeshSurface navMeshSurface;

        private void Start()
        {
            Instantiate(mapData.map, new Vector3(0.5f, 0, 10.5f), new Quaternion(0, 0, 0, 0));
            Camera.main.transform.position = new Vector3((mapData.xSize / 2) + 0.5f, 15, (mapData.ySize / 2) + 0.5f);
            
            navMeshSurface.BuildNavMesh();
            
            GameHandler.current.InitializeGame(mapData);
        }

        private void Update()
        {
            navMeshSurface.BuildNavMesh();
        }
        
        
        
        
        
        
    }
}