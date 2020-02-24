using System.Collections.Generic;
using UnityEngine;

public class PlayerMove
{
    public readonly PlayerColor PlayerColor;
    public readonly Direction Direction;
    
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