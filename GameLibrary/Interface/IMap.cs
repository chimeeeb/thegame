namespace GameLibrary.Interface
{
    /// <summary>
    /// Basic interface representing all kinds of maps existing in the game.
    /// </summary>
    public interface IMap
    {
        int Width { get; }
        int Height { get; }
        int GoalAreaHeight { get; }
    }
}