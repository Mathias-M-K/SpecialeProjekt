using System.Collections.Generic;
using Container;
using CoreGame.Strategies.Interfaces;

namespace CoreGame.Strategies.Implementations.PlayerFinishImplementations
{
    public class AllowTradePlayerFinishStrategy : PlayerFinishStrategy
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