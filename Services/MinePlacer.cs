using saper1.Entities;
using saper1.IServices;
using System.Windows.Controls;

namespace saper1.Services
{
    internal class MinePlacer : IMinePlacer
    {
        private static readonly Random _rand = new();

        public void PlaceMines(int gridSize, int minesNeeded, int safeRow, int safeCol, ref Cell[,] texts)
        {
            var availablePositions = new List<(int, int)>();

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (Math.Abs(i - safeRow) <= 1 && Math.Abs(j - safeCol) <= 1)
                        continue;

                    availablePositions.Add((i, j));
                }
            }

            var shuffled = availablePositions.OrderBy(_ => _rand.Next()).ToList();

            for (int k = 0; k < minesNeeded && k < shuffled.Count; k++)
            {
                var (i, j) = shuffled[k];

                var cell = texts[i, j];

                if (cell != null)
                {
                    cell.IsMine = true;

                    if (cell.Border.Child is TextBlock block)
                    {
                        block.Text = "M";
                    }
                }
            }
        }
    }
}
