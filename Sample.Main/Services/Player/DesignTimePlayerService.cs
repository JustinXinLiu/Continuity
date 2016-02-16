using Sample.Main.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Sample.Main.Services
{
    public class DesignTimePlayerService : IPlayerService
    {
        #region Fields

        static string _urlBase = "ms-appx:///Assets/Images/";

        static Player _curry = new Player
        {
            Id = 0,
            FirstName = "Sptephen",
            LastName = "Curry",
            Position = Position.PointerGuard,
            AvatarUrl = $"{_urlBase}stephen_curry.png",
            PointsPerGame = 29.8d,
            ReboundsPerGame = 5.3d,
            AssistsPerGame = 6.4d,
            PlayerImpactEstimate = 20.4d
        };

        static Player _james = new Player
        {
            Id = 1,
            FirstName = "LeBron",
            LastName = "James",
            Position = Position.SmallForward,
            AvatarUrl = $"{_urlBase}lebron_james.png",
            PointsPerGame = 24.9d,
            ReboundsPerGame = 7.2d,
            AssistsPerGame = 6.4d,
            PlayerImpactEstimate = 18.6d
        };

        static Player _harden = new Player
        {
            Id = 2,
            FirstName = "James",
            LastName = "Harden",
            Position = Position.ScoringGuard,
            AvatarUrl = $"{_urlBase}james_harden.png",
            PointsPerGame = 27.9d,
            ReboundsPerGame = 6.2d,
            AssistsPerGame = 7.1d,
            PlayerImpactEstimate = 17.1d
        };

        static Player _davis = new Player
        {
            Id = 3,
            FirstName = "Anthony",
            LastName = "Davis",
            Position = Position.PowerForward,
            AvatarUrl = $"{_urlBase}anthony_davis.png",
            PointsPerGame = 23d,
            ReboundsPerGame = 10.3d,
            AssistsPerGame = 1.9d,
            PlayerImpactEstimate = 16.2d
        };

        static Player _westbrook = new Player
        {
            Id = 4,
            FirstName = "Russell",
            LastName = "Westbrook",
            Position = Position.PointerGuard,
            AvatarUrl = $"{_urlBase}russell_westbrook.png",
            PointsPerGame = 24d,
            ReboundsPerGame = 7.6d,
            AssistsPerGame = 10d,
            PlayerImpactEstimate = 19.4d
        };

        static Player _durant = new Player
        {
            Id = 5,
            FirstName = "Kevin",
            LastName = "Durant",
            Position = Position.SmallForward,
            AvatarUrl = $"{_urlBase}kevin_durant.png",
            PointsPerGame = 27.4d,
            ReboundsPerGame = 8d,
            AssistsPerGame = 4.5d,
            PlayerImpactEstimate = 19.2d
        };

        static Player _griffin = new Player
        {
            Id = 6,
            FirstName = "Blake",
            LastName = "Griffin",
            Position = Position.PowerForward,
            AvatarUrl = $"{_urlBase}blake_griffin.png",
            PointsPerGame = 23.2d,
            ReboundsPerGame = 8.7d,
            AssistsPerGame = 5d,
            PlayerImpactEstimate = 17.4d
        };

        static Player _cousins = new Player
        {
            Id = 7,
            FirstName = "DeMarcus",
            LastName = "Cousins",
            Position = Position.Center,
            AvatarUrl = $"{_urlBase}demarcus_cousins.png",
            PointsPerGame = 27.1d,
            ReboundsPerGame = 11.3d,
            AssistsPerGame = 2.9d,
            PlayerImpactEstimate = 15.8d
        };

        static Player _george = new Player
        {
            Id = 8,
            FirstName = "Paul",
            LastName = "George",
            Position = Position.SmallForward,
            AvatarUrl = $"{_urlBase}paul_george.png",
            PointsPerGame = 23.1d,
            ReboundsPerGame = 7d,
            AssistsPerGame = 4d,
            PlayerImpactEstimate = 14.5d
        };

        static Player _drummond = new Player
        {
            Id = 9,
            FirstName = "Andre",
            LastName = "Drummond",
            Position = Position.Center,
            AvatarUrl = $"{_urlBase}andre_drummond.png",
            PointsPerGame = 17.3d,
            ReboundsPerGame = 15.1d,
            AssistsPerGame = .8d,
            PlayerImpactEstimate = 14.4
        };

        List<Player> _topTenPlayers = new List<Player> {
            _curry, _james, _harden, _davis,
            _westbrook, _durant, _griffin,
            _cousins, _george, _drummond
        };

        List<Player> _myFavoritePlayers = new List<Player> {
            _westbrook
        };

        List<Player> _mvpCandidates = new List<Player> {
            _curry, _james
        };

        #endregion

        public Task<IList<Player>> GetTopTenPlayersAsync()
        {
            return Task.FromResult<IList<Player>>(_topTenPlayers);
        }

        public Task<IList<Player>> GetMyFavoritePlayersAsync()
        {
            return Task.FromResult<IList<Player>>(_myFavoritePlayers);
        }

        public Task<IList<Player>> GetMvpCandidatesAsync()
        {
            return Task.FromResult<IList<Player>>(_mvpCandidates);
        }
        public Task<Player> GetPlayerAsync(long playerId)
        {
            var player = _topTenPlayers.Single((p) => p.Id == playerId);
            return Task.FromResult(player);
        }
    }
}
