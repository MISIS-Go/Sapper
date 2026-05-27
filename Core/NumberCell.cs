using Model;

namespace Core
{
    public class NumberCell : CellBase
    {
        public override bool IsMine => false;

        public NumberCell(int nearbyMines)
        {
            NearbyMinesCount = nearbyMines;
        }
    }
}
