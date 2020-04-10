using UnityEngine;

[CreateAssetMenu(fileName = "New Map",menuName = "Map")]
public class MapData : ScriptableObject
{
    [Header("Map Prefab")] 
    public GameObject map;
    
    [Header("Camera Data")] 
    public Vector3 cameraPos;
    
    [Header("Map Info")] 
    public int xSize;
    public int ySize;
        
    [Header("Game Info")] 
    public int maxPlayers;
    public Vector2[] spawnPositions;

    
}