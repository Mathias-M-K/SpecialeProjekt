using CoreGame;
using UnityEngine;

namespace Container
{
    public class StoredPlayerMove : MonoBehaviour
    {
        public Player player;
        public Direction direction;
    
        public StoredPlayerMove(Player pc,Direction d)
        {
            player = pc;
            direction = d;
        }
    }

}