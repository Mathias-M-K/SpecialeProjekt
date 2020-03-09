using Container;

namespace CoreGame.Interfaces
{
    public interface IStatObserver
    {
        //Moves
        void NewMoveAdded(StoredPlayerMove move);
        
        
        //Trades
        //Status: Pending, Accepted, Rejected, Canceled
        void NewTradeActivity(PlayerTrade playerTrade,string status);

    }
}