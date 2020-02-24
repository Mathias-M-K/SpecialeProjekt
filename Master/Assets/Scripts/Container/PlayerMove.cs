using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove
{
    public PlayerColor PlayerColor;
    public Direction Direction;
    
    public PlayerMove(PlayerColor pc,Direction d)
    {
        PlayerColor = pc;
        Direction = d;
    }
}
public enum PlayerColor
{
    Red,
    Blue,
    Green,
    Yellow
}
