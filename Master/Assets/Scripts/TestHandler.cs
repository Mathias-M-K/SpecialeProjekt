using System.Collections.Generic;
using ArduinoUnityConnection;
using Container;
using CoreGame;
using UnityEngine;


public class TestHandler : MonoBehaviour
{
    [Header("Wireless Connection")] public string ipAdress;
    public int port;
    public List<PlayerTrade> playerTrades = new List<PlayerTrade>();
    [SerializeField] private float IncommingValue;

    [Space] [Header("Other")] [Range(0f, 3f)] [SerializeField]
    private float sequenceDelay;

    public PlayerController agent1;
    public PlayerController agent2;


    public GameHandler gameHandler;

    private bool _serverActive;
    private WifiConnection _wifiConnection = new WifiConnection();

    private void Start()
    {
        gameHandler.AddMoveToSequece(Player.Yellow, Direction.Left);

        gameHandler.AddMoveToSequece(Player.Red, Direction.Down);

        gameHandler.AddMoveToSequece(Player.Blue, Direction.Up);
        gameHandler.AddMoveToSequece(Player.Blue, Direction.Up);
        gameHandler.AddMoveToSequece(Player.Blue, Direction.Up);
        gameHandler.AddMoveToSequece(Player.Blue, Direction.Right);

        gameHandler.AddMoveToSequece(Player.Green, Direction.Up);

        gameHandler.AddMoveToSequece(Player.Yellow, Direction.Left);
        gameHandler.AddMoveToSequece(Player.Yellow, Direction.Left);

        gameHandler.AddMoveToSequece(Player.Red, Direction.Down);

        gameHandler.AddMoveToSequece(Player.Green, Direction.Up);

        gameHandler.AddMoveToSequece(Player.Red, Direction.Right);

        gameHandler.AddMoveToSequece(Player.Green, Direction.Left);

        gameHandler.AddMoveToSequece(Player.Red, Direction.Right);

        gameHandler.AddMoveToSequece(Player.Yellow, Direction.Down);

        gameHandler.AddMoveToSequece(Player.Green, Direction.Left);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            agent1.MovePlayer(Direction.Left);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            agent1.MovePlayer(Direction.Right);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            agent1.MovePlayer(Direction.Up);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            agent1.MovePlayer(Direction.Down);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(gameHandler.PerformSequence(sequenceDelay));
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            List<Vector3> positions = gameHandler.GetSpawnLocations();

            int i = 0;
            foreach (PlayerController playerController in gameHandler.GetPlayers())
            {
                playerController.MoveToPos(positions[i].x, positions[i].z);
                i++;
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            _wifiConnection.Begin(ipAdress, port);
            _serverActive = true;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            gameHandler.OfferMove(Direction.Up,Player.Red,Player.Blue);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            agent1.AcceptTradeFrom(Player.Blue,Direction.Right);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            agent1.RejectTradeFrom(Player.Blue);
        }
        
        if (_serverActive)
        {
            IncommingValue = _wifiConnection.CurrentValue;
        }
    }
}