using Container;

public interface ITradeObserver
{
    void TradeUpdate(PlayerTrade playerTrade ,TradeActions tradeAction);
}

public enum TradeActions
{
    TradeOffered,
    TradeRejected,
    TradeAccepted,
    TradeCanceled
}
