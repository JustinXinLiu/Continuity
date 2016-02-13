using Sample.Main.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sample.Main.Services
{
    public partial class PlayerService : IPlayerService
    {
        public Task<IList<Player>> GetTopTenPlayersAsync()
        {
            throw new NotImplementedException();
        }
        public Task<IList<Player>> GetMyFavoritePlayersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IList<Player>> GetMvpCandidatesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Player> GetPlayerAsync(long playerId)
        {
            throw new NotImplementedException();
        }

    }
}
