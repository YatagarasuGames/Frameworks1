using Frameworks1.DTOs;
using System.Collections.Concurrent;

namespace Frameworks1.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {

        private readonly ConcurrentDictionary<Guid, Player> _storage = new();

        public List<Player> GetAll()
        {
            return _storage.Select(p => new Player { Id = p.Value.Id, Name = p.Value.Name, Score = p.Value.Score }).ToList();
        }

        public Player? GetById(Guid id)
        {
            return _storage.TryGetValue(id, out var player) ? player : null;
        }

        public Player Create(string name, int score)
        {
            Guid id = Guid.NewGuid();
            Player player = new Player { Id = id, Name = name, Score = score };

            _storage[id] = player;
            return player;
        }
    }
}
