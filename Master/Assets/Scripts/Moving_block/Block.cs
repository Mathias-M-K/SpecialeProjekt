using System;
using System.Collections;
using System.Collections.Generic;
using CoreGame;
using UnityEngine;

public class Block
{
    private string _id;
    private Direction _state;
    private PlayerTags _playerTags;
    
    public Block(string id, Direction state, PlayerTags playerTags)
    {
        _id = id;
        _state = state;
        _playerTags = playerTags;
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

    public PlayerTags playerTags
    {
        get => _playerTags;
        set => _playerTags = value;
    }
}
