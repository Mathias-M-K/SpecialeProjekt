using CoreGame;

public interface IInventoryObserver
{
    void OnMoveInventoryChange(Direction[] directions);
}
