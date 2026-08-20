using saper1.Entities;
using saper1.Services;

namespace saper1.IServices
{
    public interface IGridBuilder
    {
        void BuildGrid(GridBuilderOptions<Cell> options);
        void PlaceMines(GridBuilderOptions<Cell> options, int mineCount, int safeRow, int safeCol);
        void CountMines(GridBuilderOptions<Cell> options);
    }
}
