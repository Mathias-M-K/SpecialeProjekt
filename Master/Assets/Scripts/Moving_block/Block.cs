using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private string _id;
    private string _state;
    private string _currentState;
    public Block(string id, string state)
    {
        _id = id;
        _state = state;
        _currentState = state;
    }

    public string Id
    {
        get => _id;
        set => _id = value;
    }

    public string State
    {
        get => _state;
        set => _state = value;
    }

    public string CurrentState
    {
        get => _currentState;
        set => _currentState = value;
    }
}
