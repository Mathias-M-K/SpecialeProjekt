using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public PlayerColor pc;
    public Direction d;
    
    public PlayerMove(PlayerColor pc,Direction d)
    {
        this.pc = pc;
        this.d = d;
    }
}
public enum PlayerColor
{
    UP,
    DOWN,
    RIGHT,
    LEFT
}
