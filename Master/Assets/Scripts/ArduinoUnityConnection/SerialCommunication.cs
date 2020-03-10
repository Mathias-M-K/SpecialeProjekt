using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Reflection;
using Container;
using CoreGame;
using CoreGame.Interfaces;
using UnityEngine.Serialization;

public class SerialCommunication : MonoBehaviour,ISequenceObserver
{
    public string port;
    private SerialPort _stream;
    private BlockHandler _block;
    private string _dataStream = "";
    public string id = "";
    private string _oldString = "";
    public string methodSelected;
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
            if (!_dataStream.Equals("") && !_dataStream.Equals(_oldString))
            {
                _oldString = _dataStream;
                string[] tempStringArray = _dataStream.Split(':');
                id = tempStringArray[1];
                id = id.Trim();
                methodSelected = tempStringArray[0];
                if (methodSelected.Equals("add"))
                {
                    _block.AddBlock(id);    
                }

                if (methodSelected.Equals("swap"))
                {
                    //TODO: trigger swap function
                    _playerController.CreateTrade(_block.GetDirectionFromId(id),Player.Blue);
                }

                if (methodSelected.Equals("sequence"))
                {
                    //TODO: trigger add move to sequence
                    _playerController.gameHandler.AddMoveToSequence(_playerController.player,_block.GetDirectionFromId(id));
                }
                
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    

    private void OnApplicationQuit()
    {
        _stream.Close();
    }

    public void Begin(string portNr)
    {
        _playerController = GetComponent<PlayerController>();
        _playerController.gameHandler.AddSequenceObserver(this);
        
        
        _stream = new SerialPort(portNr, 9600,Parity.Even,7,StopBits.One);
        _stream.ReadTimeout = 1;
        _stream.Open(); //Open the Serial Stream.
        _block = gameObject.AddComponent<BlockHandler>();
        _isEnabled = true;
    }

    public void SequenceUpdate(SequenceActions sequenceAction, StoredPlayerMove move)
    {
        switch (sequenceAction)
        {
            case SequenceActions.NewMoveAdded:
                break;
            case SequenceActions.MoveRemoved:
                break;
            case SequenceActions.SequencePlayed:
                _oldString = "";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(sequenceAction), sequenceAction, null);
        }
    }
}

