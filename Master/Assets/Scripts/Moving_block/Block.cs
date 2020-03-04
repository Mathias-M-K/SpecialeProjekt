using System;
using System.Collections;
using System.Collections.Generic;
using CoreGame;
using UnityEngine;

public class Block
{
    private string _id;
    private Direction _state;
    private Player _player;
    
    public Block(string id, Direction state, Player player)
    {
        _id = id;
        _state = state;
        _player = player;
    }

    public string Id
    {
        get => _id;
        set => _id = value;
    }

    public Direction State
    {
        get => _state;
        set => _state = value;
    }

    public Player player
    {
        get => _player;
        set => _player = value;
    }
}
