namespace GameLibrary.Interface
{
    /// <summary>
    /// An extension of IMap to represent a map with tiles of given type.
    /// </summary>
    /// <typeparam name="T">Tile type.</typeparam>
    public interface IMap<T> : IMap where T : class, ITile
    {
        T this[int i, int j] { get; set; }
    }
}