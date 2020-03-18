using System.Collections.Generic;
using ArduinoUnityConnection;
using CoreGame;
using UnityEngine;


public class TestHandler : MonoBehaviour
{
    [Header("Wireless Connection")] public string ipAdress;
    public int port;
    public string outgoingString;
    public float incomingValue;

    [Space] [Header("Other")] [Range(0f, 3f)] [SerializeField]
    private float sequenceDelay;

    public PlayerController agent1;
    public PlayerController agent2;
    
    private bool _serverActive;


    private readonly WifiConnection _wifiConnection = new WifiConnection();


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
            GameHandler.current.delayBetweenMoves = sequenceDelay;
            StartCoroutine(GameHandler.current.PerformSequence());
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Vector2[] positions = GameHandler.current.GetSpawnLocations();

            int i = 0;
            foreach (PlayerController playerController in GameHandler.current.GetPlayers())
            {
                playerController.MoveToPos(positions[i].x, positions[i].y);
                i++;
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            _wifiConnection.Begin(ipAdress, port);

            _serverActive = true;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            _wifiConnection.WriteToArduino(outgoingString);
        }

        if (_serverActive)
        {
            incomingValue = _wifiConnection.CurrentValue;
        }
    }
}