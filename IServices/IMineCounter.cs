using saper1.Entities;

namespace saper1.IServices
{
    public interface IMineCounter
    {
        void CountAllMines(ref Cell[,] _cells);
    }
}
