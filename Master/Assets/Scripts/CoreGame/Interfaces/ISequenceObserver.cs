using Container;

namespace CoreGame.Interfaces
{
    public interface ISequenceObserver
    {
        void OnSequenceChange(SequenceActions sequenceAction, StoredPlayerMove move);
    }
}

public enum SequenceActions
{
    NewMoveAdded,
    MoveRemoved,
    SequenceStarted,
    SequenceEnded,
    MovePerformed
}
