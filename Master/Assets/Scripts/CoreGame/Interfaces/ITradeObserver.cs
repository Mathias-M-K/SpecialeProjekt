using Container;

public interface ITradeObserver
{
    void OnNewTradeActivity(PlayerTrade playerTrade ,TradeActions tradeAction);
}

public enum TradeActions
{
    TradeOffered,
    TradeRejected,
    TradeAccepted,
    TradeCanceled,
    TradeCanceledByGameHandler
}
