using CoreGame;
using UnityEngine;

namespace Container
{
    public class StoredPlayerMove
    {
        public PlayerTags PlayerTags;
        public Direction Direction;
    
        public StoredPlayerMove(PlayerTags pc,Direction d)
        {
            PlayerTags = pc;
            Direction = d;
        }
    }

}