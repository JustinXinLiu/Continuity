#region Usings

using Sample.Main.Models;
using Sample.Main.Mvvm;
using Sample.Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;

#endregion

namespace Sample.Main.ViewModels
{
    public class PlayerViewModel : ViewModelBase
    {
        #region fields

        private Player _player;
        private IPlayerService _playerService;

        #endregion

        #region Constructors

        public PlayerViewModel(IPlayerService playerService)
        {
            if (IsInDesignMode)
            {
                Player = new Player();
            }
            else
            {
                _playerService = playerService;
            }
        }

        #endregion

        #region Overrides

        public async override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            var id = long.Parse(parameter.ToString());
            Player = await _playerService.GetPlayerAsync(id);
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending)
        {
            return base.OnNavigatedFromAsync(state, suspending);
        }

        #endregion

        #region Properties

        public Player Player { get { return _player; } set { Set(ref _player, value); } }

        #endregion

        #region Methods

        #endregion
    }
}
