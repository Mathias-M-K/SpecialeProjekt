using System.Collections.Generic;
using Container;
using CoreGame.Strategies.Interfaces;

namespace CoreGame.Strategies.PlayerFinishImplementations
{
    public class AllowTradePlayerFinishStrategy : _PlayerFinishStrategy
    {
        public void PlayerFinish(PlayerController playerController)
        {
            
            List<StoredPlayerMove> moves = new List<StoredPlayerMove>(GameHandler.current.GetSequence());
            //Removing all moves from the common sequence
            foreach (StoredPlayerMove move in moves)
            {
                if (move.Player == playerController.player)
                {
                    GameHandler.current.RemoveMoveFromSequence(move);
                }
            }
            
            playerController.DestroySelf();
        }
    }
}