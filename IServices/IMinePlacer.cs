using saper1.Entities;

namespace saper1.IServices
{
    public interface IMinePlacer
    {
        public void PlaceMines(int gridSize, int mineProbability, int safeRow, int safeCol, ref Cell[,] texts);
    }
}
