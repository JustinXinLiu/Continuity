using Sample.Main.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Main.Services
{
    public interface IPlayerService
    {
        Task<IList<Player>> GetTopTenPlayersAsync();

        Task<IList<Player>> GetMyFavoritePlayersAsync();

        Task<IList<Player>> GetMvpCandidatesAsync();

        Task<Player> GetPlayerAsync(long playerId);
    }
}
