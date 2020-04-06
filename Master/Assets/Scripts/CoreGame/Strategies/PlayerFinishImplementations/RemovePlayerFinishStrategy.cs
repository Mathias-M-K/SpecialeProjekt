using System.Collections.Generic;
using Container;
using CoreGame.Strategies.Interfaces;

namespace CoreGame.Strategies.Implementations.PlayerFinishImplementations
{
    public class RemovePlayerFinishStrategy : _PlayerFinishStrategy 
    {
        public void PlayerFinish(PlayerController playerController)
        {
            //Rejecting all trade offers
            List<PlayerTrade> incomingTradesTemp = new List<PlayerTrade>(playerController.incomingTradeOffers);
            foreach (PlayerTrade trade in incomingTradesTemp)
            {
                trade.RejectTrade(playerController);
            }
            
            //Cancelling all trades by the player
            List<PlayerTrade> outgoingTradesTemp = new List<PlayerTrade>(playerController.outgoingTradeOffers);
            foreach (PlayerTrade trade in outgoingTradesTemp)
            {
                trade.CancelTrade(playerController.playerTags);
            }

            List<StoredPlayerMove> moves = new List<StoredPlayerMove>(GameHandler.current.GetSequence());
            
            //Remove all moves from player, so they can't continue to play
            for (int i = 0; i < 4; i++)
            {
                playerController.RemoveMove(i);
            }
            
            //Removing all moves from the common sequence
            foreach (StoredPlayerMove move in moves)
            {
                if (move.PlayerTags == playerController.playerTags)
                {
                    GameHandler.current.RemoveMoveFromSequence(move);
                }
            }
            
            GameHandler.current.RemovePlayerController(playerController);
            playerController.DestroySelf();
        }
    }
}