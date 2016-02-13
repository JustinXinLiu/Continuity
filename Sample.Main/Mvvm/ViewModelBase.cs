using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;

namespace Sample.Main.Mvvm
{
    public abstract class ViewModelBase :
         GalaSoft.MvvmLight.ViewModelBase, INavigable
    {
        #region Overrides

        public virtual Task OnNavigatedFromAsync(IDictionary<string, object> state, bool suspending) => Task.CompletedTask;

        public virtual Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state) => Task.CompletedTask;

        public virtual Task OnNavigatingFromAsync(NavigatingEventArgs args) => Task.CompletedTask;

        #endregion

        #region Properties

        [JsonIgnore]
        public INavigationService NavigationService { get; set; }

        [JsonIgnore]
        public Template10.Common.IDispatcherWrapper Dispatcher { get; set; }

        [JsonIgnore]
        public Template10.Common.IStateItems SessionState { get; set; }

        #endregion
    }
}
