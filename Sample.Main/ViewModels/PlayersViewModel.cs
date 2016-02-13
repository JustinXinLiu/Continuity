#region Usings

using GalaSoft.MvvmLight.Command;
using Sample.Main.Common;
using Sample.Main.Models;
using Sample.Main.Mvvm;
using Sample.Main.Services;
using Sample.Main.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;

#endregion

namespace Sample.Main.ViewModels
{
    public class PlayersViewModel : ViewModelBase, IAsyncInitialization
    {
        #region fields

        private RelayCommand<long> _navigateCommand;

        #endregion

        #region Constructors

        public PlayersViewModel(IPlayerService playerService)
        {
            if (IsInDesignMode)
            {
                TopTenPlayers = new ObservableCollection<Player>(playerService.GetTopTenPlayersAsync().Result);
                MyFavoritePlayers = new ObservableCollection<Player>(playerService.GetMyFavoritePlayersAsync().Result);
                MvpCandidates = new ObservableCollection<Player>(playerService.GetMvpCandidatesAsync().Result);
            }
            else
            {
                Initialization = InitializeAsync(playerService);
            }
        }

        #endregion

        #region Overrides

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            return Task.CompletedTask;
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            return base.OnNavigatedFromAsync(state, suspending);
        }

        #endregion

        #region Properties

        public Task Initialization { get; private set; }

        public ObservableCollection<Player> TopTenPlayers { get; } = new ObservableCollection<Player>();

        public ObservableCollection<Player> MyFavoritePlayers { get; } = new ObservableCollection<Player>();

        public ObservableCollection<Player> MvpCandidates { get; } = new ObservableCollection<Player>();

        public RelayCommand<long> NavigateCommand
        {
            get
            {
                return _navigateCommand ?? (_navigateCommand = new RelayCommand<long>((id) =>
                {
                    NavigationService.Navigate(typeof(PlayerView), id);
                }));
            }
        }

        #endregion

        #region Methods

        private async Task InitializeAsync(IPlayerService playerService)
        {
            // Fake the service call
            await Task.Delay(1000);

            var topTenPlayers = await playerService.GetTopTenPlayersAsync();
            // add each game to the collection in a timely fashion
            //Players.AddRange(players);
            foreach (var item in topTenPlayers)
            {
                TopTenPlayers.Add(item);
            }

            var myFavoritePlayers = await playerService.GetMyFavoritePlayersAsync();
            // add each game to the collection in a timely fashion
            //Players.AddRange(players);
            foreach (var item in myFavoritePlayers)
            {
                MyFavoritePlayers.Add(item);
            }

            var mvpCandidates = await playerService.GetMvpCandidatesAsync();
            // add each game to the collection in a timely fashion
            //Players.AddRange(players);
            foreach (var item in mvpCandidates)
            {
                MvpCandidates.Add(item);
            }
        }

        #endregion
    }
}
