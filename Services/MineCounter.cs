using saper1.Entities;
using saper1.Extensions;
using saper1.IServices;

namespace saper1.Services
{
    public class MineCounter : IMineCounter
    {
        public void CountAllMines(ref Cell[,] _cells)
        {
            var mineCells = _cells.Where(c => c.IsMine).ToList();

            foreach (var mine in mineCells)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        int nx = mine.Coordinates.X + dx;
                        int ny = mine.Coordinates.Y + dy;

                        if (nx < 0 || ny < 0 || nx >= _cells.GetLength(0) || ny >= _cells.GetLength(1))
                            continue;

                        var neighbor = _cells[nx, ny];

                        if (neighbor == null || neighbor.IsMine)
                            continue;

                        neighbor.AdjacentMines++;
                    }
                }
            }
        }

    }
}
