using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using CoreGame;
using UnityEngine.Serialization;

public class SerialCommunication : MonoBehaviour
{
    public string port;
    private SerialPort _stream;
    private BlockHandler _block;
    private string _dataStream = "";
    public bool swapIt;
    public string id = "";
    public string swapId = "";
    private PlayerController _playerController;

    private bool _isEnabled;
    public bool begin;
    

    private void Update()
    {
        if (begin)
        {
            Begin(port);
            begin = false;
        }

        if (!isActiveAndEnabled) return;
        try
        {
            _dataStream = _stream.ReadExisting();
            if (!_dataStream.Equals("") && !_dataStream.Equals(id))
            {
                id = _dataStream;
                _block.AddBlock(id);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    

    private void OnApplicationQuit()
    {
        if (_stream.IsOpen)
        {
            _stream.Close();
        }
    }

    public void Begin(string portNr)
    {
        _playerController = GetComponent<PlayerController>();
        
        
        _stream = new SerialPort(portNr, 9600,Parity.Even,7,StopBits.One);
        _stream.ReadTimeout = 1;
        _stream.Open(); //Open the Serial Stream.
        _block = gameObject.AddComponent<BlockHandler>();
        _isEnabled = true;
    }
}

