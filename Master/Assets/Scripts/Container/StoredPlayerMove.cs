using CoreGame;
using UnityEngine;

namespace Container
{
    public class StoredPlayerMove
    {
        public Player Player;
        public Direction Direction;
    
        public StoredPlayerMove(Player pc,Direction d)
        {
            Player = pc;
            Direction = d;
        }
    }

}