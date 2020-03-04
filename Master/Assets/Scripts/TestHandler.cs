using System;
using System.Collections.Generic;
using ArduinoUnityConnection;
using Container;
using CoreGame;
using UnityEngine;


public class TestHandler : MonoBehaviour
{
    [Header("Wireless Connection")] public string ipAdress;
    public int port;
    public WifiMethod wifiMethod;
    public string outgoingString;
    [SerializeField]private float _incommingValue;

    [Space] [Header("Other")] [Range(0f, 3f)] [SerializeField]
    private float sequenceDelay;

    public PlayerController agent1;
    public PlayerController agent2;


    public GameHandler gameHandler;

    private bool _serverActive;

    
    private readonly WifiConnectionImproved _wifiConnectionImproved = new WifiConnectionImproved();
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
            switch (wifiMethod)
            {
                case WifiMethod.Old:
                    _wifiConnection.Begin(ipAdress, port);
                    break;
                case WifiMethod.New:
                    _wifiConnectionImproved.Begin(ipAdress,port);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _serverActive = true;
        }
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            _wifiConnectionImproved.WriteToArduino(outgoingString);
            
            //wifiConnectionImproved.OutgoingData = outgoingString;
            //wifiConnectionImproved._outgoingDataAvailable = true;
        }
        
        if (_serverActive)
        {
            switch (wifiMethod)
            {
                case WifiMethod.Old:
                    _incommingValue = _wifiConnection.CurrentValue;
                    break;
                case WifiMethod.New:
                    _incommingValue = _wifiConnectionImproved.CurrentValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void OnApplicationQuit()
    {
        _wifiConnectionImproved.CloseConnection();
    }
}

public enum WifiMethod
{
    Old,
    New
}