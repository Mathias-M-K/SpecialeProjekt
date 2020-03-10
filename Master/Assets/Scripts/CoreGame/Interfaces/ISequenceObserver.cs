using Container;

namespace CoreGame.Interfaces
{
    public interface ISequenceObserver
    {
        void SequenceUpdate(SequenceActions sequenceAction, StoredPlayerMove move);
    }
}

public enum SequenceActions
{
    NewMoveAdded,
    MoveRemoved,
    SequencePlayed
}
