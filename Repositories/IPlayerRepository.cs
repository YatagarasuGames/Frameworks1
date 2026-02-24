using Frameworks1.DTOs;

namespace Frameworks1.Repositories
{
    public interface IPlayerRepository
    {
        Player Create(string name, int score);
        List<Player> GetAll();
        Player? GetById(Guid id);
    }
}