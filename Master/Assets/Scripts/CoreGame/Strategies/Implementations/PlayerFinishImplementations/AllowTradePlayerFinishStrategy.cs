using System.Collections.Generic;
using Container;
using CoreGame.Strategies.Interfaces;

namespace CoreGame.Strategies.Implementations.PlayerFinishImplementations
{
    public class AllowTradePlayerFinishStrategy : PlayerFinishStrategy
    {
        public void PlayerFinish(PlayerController playerController, GameHandler gameHandler)
        {
            
            List<StoredPlayerMove> moves = new List<StoredPlayerMove>(gameHandler.GetSequence());
            //Removing all moves from the common sequence
            foreach (StoredPlayerMove move in moves)
            {
                if (move.Player == playerController.player)
                {
                    gameHandler.RemoveMoveFromSequence(move);
                }
            }
            
            playerController.DestroySelf();
        }
    }
}