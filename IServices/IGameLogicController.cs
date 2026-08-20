using saper1.Entities;

namespace saper1.IServices
{
    public interface IGameLogicController
    {
        void RevealAllMines(List<Cell> mineMap);
    }
}
