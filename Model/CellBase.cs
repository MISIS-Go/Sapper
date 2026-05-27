namespace Model
{
    public abstract class CellBase : ICell
    {
        public bool IsOpened { get; set; }
        public bool HasFlag { get; set; }
        public abstract bool IsMine { get; }
        public int NearbyMinesCount { get; set; }

        protected CellBase()
        {
            IsOpened = false;
            HasFlag = false;
            NearbyMinesCount = 0;
        }
    }
}
