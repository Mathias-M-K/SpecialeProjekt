using System.Collections.Generic;
using Container;
using CoreGame.Strategies.Interfaces;

namespace CoreGame.Strategies.PlayerFinishImplementations
{
    public class AllowTradePlayerFinishStrategy : _PlayerFinishStrategy
    {
        public void PlayerFinish(PlayerController playerController)
        {
            
            List<StoredPlayerMove> moves = new List<StoredPlayerMove>(GameHandler.Current.GetSequence());
            //Removing all moves from the common sequence
            foreach (StoredPlayerMove move in moves)
            {
                if (move.PlayerTags == playerController.playerTag)
                {
                    GameHandler.Current.RemoveMoveFromSequence(move);
                }
            }
            
            playerController.DestroySelf();
        }
    }
}