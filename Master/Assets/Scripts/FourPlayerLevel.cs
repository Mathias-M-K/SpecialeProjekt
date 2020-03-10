using UnityEngine;

[CreateAssetMenu(fileName = "New Map",menuName = "Map")]
public class FourPlayerLevel : ScriptableObject
{
    [Header("Map Prefab")] public GameObject map;
    
    [Space] [Header("Map Info")] 
    public int xSize;
    public int ySize;
        
    [Space] [Header("Game Info")] 
    public int maxPlayers;
    public Vector2[] spawnPositions;

}