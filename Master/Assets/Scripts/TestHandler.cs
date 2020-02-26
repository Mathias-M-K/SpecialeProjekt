using System.Collections.Generic;
using ArduinoUnityConnection;
using CoreGame;
using UnityEngine;


public class TestHandler : MonoBehaviour
{
    [Header("Wireless Connection")] 
    public string ipAdress;
    public int port;
    [SerializeField] private float IncommingValue;
    
    [Space][Header("Other")] [Range(0f, 3f)] [SerializeField] private float sequenceDelay;
    public PlayerController agent;
    

    public GameHandler gameHandler;

    private bool _serverActive;
    private WifiConnection _wifiConnection = new WifiConnection();

    private void Start()
    {
        gameHandler.AddMove(Player.Yellow,Direction.Left);
        
        gameHandler.AddMove(Player.Red,Direction.Down);
        
        gameHandler.AddMove(Player.Blue,Direction.Up);
        gameHandler.AddMove(Player.Blue,Direction.Up);
        gameHandler.AddMove(Player.Blue,Direction.Up);
        gameHandler.AddMove(Player.Blue,Direction.Right);
        
        gameHandler.AddMove(Player.Green,Direction.Up);
        
        gameHandler.AddMove(Player.Yellow,Direction.Left);
        gameHandler.AddMove(Player.Yellow,Direction.Left);
        
        gameHandler.AddMove(Player.Red,Direction.Down);
        
        gameHandler.AddMove(Player.Green,Direction.Up);
        
        gameHandler.AddMove(Player.Red,Direction.Right);
        
        gameHandler.AddMove(Player.Green,Direction.Left);
        
        gameHandler.AddMove(Player.Red,Direction.Right);
        
        gameHandler.AddMove(Player.Yellow,Direction.Down);
        
        gameHandler.AddMove(Player.Green,Direction.Left);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            agent.MovePlayer(Direction.Left);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            agent.MovePlayer(Direction.Right);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            agent.MovePlayer(Direction.Up);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            agent.MovePlayer(Direction.Down);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(gameHandler.PerformMoves(sequenceDelay));
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            List<Vector3> positions = gameHandler.GetSpawnLocations();

            int i = 0;
            foreach (PlayerController playerController in gameHandler.GetPlayers())
            {
                playerController.MoveToPos(positions[i].x,positions[i].z);
                i++;
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            
            _wifiConnection.Begin(ipAdress,port);
            _serverActive = true;
        }

        if (_serverActive)
        {
            IncommingValue = _wifiConnection.CurrentValue;
        }
    }
}