namespace Model.Core
{
    public interface ICell
    {
        bool IsOpened { get; set; }
        bool HasFlag { get; set; }
        bool IsMine { get; }
        int NearbyMinesCount { get; set; }
    }
}