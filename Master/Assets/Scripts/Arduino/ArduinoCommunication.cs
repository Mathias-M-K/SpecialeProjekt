using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine.Serialization;

public class ArduinoCommunication : MonoBehaviour {

    SerialPort stream = new SerialPort("COM5", 9600,Parity.Even,7,StopBits.One);
    private Player_handler _player;
    private string dataStream = "";
    public bool SwapIT = false;
    public string id = "";
    public string swapId = "";
    void Start()
    {
        stream.ReadTimeout = 1;
        stream.Open(); //Open the Serial Stream.
        _player = new Player_handler();
    }

    void Update()
    {
        try
        {
            dataStream = stream.ReadExisting();
            if (!dataStream.Equals("") && !dataStream.Equals(id))
            {
                id = dataStream;
                _player.AddBlock(id);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        TriggerSwap();
    }

    void TriggerSwap()
    {
        if (SwapIT)
        {
            _player.SwapBlocks(swapId,"down");
            SwapIT = !SwapIT;
        }
    }
    
    private void OnApplicationQuit()
    {
        if (stream.IsOpen)
        {
            stream.Close();
        }
    }
}
