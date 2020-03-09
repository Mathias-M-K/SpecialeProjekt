using Container;

namespace CoreGame.Interfaces
{
    public interface IStatObserver
    {
        void NewMoveAdded();
        void NewTradeAdded(PlayerTrade playerTrade);
    }
}