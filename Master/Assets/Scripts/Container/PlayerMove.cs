using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public PlayerColor player;
    public Direction direction;
    
    public PlayerMove(PlayerColor pc,Direction d)
    {
        player = pc;
        direction = d;
    }
}
public enum PlayerColor
{
    Red,
    Blue,
    Green,
    Yellow
}