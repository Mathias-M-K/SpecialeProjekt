using CoreGame;
using UnityEngine;

namespace Container
{
    public class StoredPlayerMove
    {
        public PlayerTags PlayerTags;
        public Direction Direction;
        public int moveIndex;
        public int Id;
        
    
        public StoredPlayerMove(PlayerTags pc,Direction d,int moveIndex,int id)
        {
            PlayerTags = pc;
            Direction = d;
            this.moveIndex = moveIndex;
            Id = id;
        }
    }

}