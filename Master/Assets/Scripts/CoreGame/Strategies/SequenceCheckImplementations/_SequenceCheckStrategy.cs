namespace CoreGame.Strategies.Interfaces
{
    public interface _SequenceCheckStrategy
    {
        //Implementation dictates if sequence is allowed to play
        bool SequencePlayAllowed(GameHandler gameHandler);
    }
}