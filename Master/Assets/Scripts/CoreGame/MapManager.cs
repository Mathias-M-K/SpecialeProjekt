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
        public static MapManager current;

        private void Awake()
        {
            current = this;
        }

        [Header("Level Information")] public MapData mapData;
        public NavMeshSurface navMeshSurface;

        private void Start()
        {
            Instantiate(mapData.map, new Vector3(0.5f, 0, 10.5f), new Quaternion(0, 0, 0, 0));
            if(Camera.main != null) Camera.main.transform.position = new Vector3((mapData.xSize / 2) + 0.5f, 15, (mapData.ySize / 2) + 0.5f);
            
            navMeshSurface.BuildNavMesh();

            SendMapDataToGameHandler();
            //if(!GameHandler.current.playersAreExternallyControlled) GameHandler.current.SpawnMaxPlayers();
        }

        public void SendMapDataToGameHandler()
        {
            GameHandler.current.SetMapData(mapData);
        }

        private void Update()
        {
            navMeshSurface.BuildNavMesh();
        }

        public string[,] GenerateMapValues()
        {
            /*
             * Values and meanings
             * 
             * f = floor
             * w = wall
             * r = red Player
             * b = blue player
             * g = green player
             * y = yellow player
             * xt= x trigger eg. rt = red trigger
             * xg= x wall    eg. rw = red gate
             * fp = finish point
             */
            
            string[,] mapValues = new string[mapData.xSize+1,mapData.ySize+1];
            
            //Setting all to floor
            for (int i = 1; i <= mapData.xSize; i++)
            {
                for (int j = 1; j <= mapData.ySize; j++)
                {
                    mapValues[i,j] = "f";
                }
            }

            for (int y = 1; y <= mapData.ySize; y++)
            {
                for (int x = 1; x <= mapData.xSize; x++)
                {
                    Vector3 position = new Vector3(x,3,y);
                    Ray ray = new Ray(position,Vector3.down);
                    
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        string s = hit.transform.name;
                        s = s.ToLower();
                        
                        if (s.Length > 6 && s.Substring(s.Length-7,7).Equals("trigger"))
                        {
                            mapValues[x, y] = s.Substring(0, 1) + "t";
                        }
                        if (s.Length > 4 && s.Substring(s.Length-4,4).Equals("gate"))
                        {
                            mapValues[x, y] = s.Substring(0, 1) + "g";
                        }

                        if (s.Length > 4 && s.Substring(0,4).Equals("wall"))    
                        {
                            mapValues[x, y] = "w";
                        }

                        if (s.Equals("finishpoint"))
                        {
                            mapValues[x, y] = "fp";
                        }

                        switch (s)
                        {
                            case "red":
                                mapValues[x, y] = "r";
                                break;
                            case "blue":
                                mapValues[x, y] = "b";
                                break;
                            case "green":
                                mapValues[x, y] = "g";
                                break;
                            case "yellow":
                                mapValues[x, y] = "y";
                                break;
                        }
                    }
                }
            }
            
            return mapValues;
        }
        
    }
}