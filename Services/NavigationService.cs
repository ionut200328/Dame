using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dame.Core;
using Dame.MVVM.ViewModel;

namespace Dame.Services
{
    public interface INavigationService
    {
        event Action<ViewModel> OnNavigated;
        ViewModel CurrentViewModel { get; }
        void NavigateTo<T>(object parameter = null) where T : ViewModel;
        void NavigateTo<TViewModel>() where TViewModel : ViewModel;
    }
    public class NavigationService : ObservableObject, INavigationService
    {
        private readonly Dictionary<Type, ViewModel> _viewModelInstances = new Dictionary<Type, ViewModel>();
        private ViewModel _currentView;
        private readonly Func<Type, ViewModel> _viewModelFactory;
        public event Action<Dame.Core.ViewModel> OnNavigated;

        public ViewModel CurrentViewModel
        {
            get => _currentView;
            private set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public NavigationService(Func<Type, ViewModel> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }
        public void NavigateTo<TViewModel>(object parameter) where TViewModel : ViewModel
        {
            if (!_viewModelInstances.TryGetValue(typeof(TViewModel), out var viewModel))
            {
                viewModel = _viewModelFactory.Invoke(typeof(TViewModel));
                _viewModelInstances[typeof(TViewModel)] = viewModel;
            }

            if (viewModel is GameViewModel gameViewModel && parameter is string command)
            {
                gameViewModel.SetParameter(command);
            }

            CurrentViewModel = viewModel;
            OnNavigated?.Invoke(viewModel);
        }


        public void NavigateTo<TViewModel>() where TViewModel : ViewModel
        {
            if (!_viewModelInstances.TryGetValue(typeof(TViewModel), out var viewModel))
            {
                viewModel = _viewModelFactory.Invoke(typeof(TViewModel));
                _viewModelInstances[typeof(TViewModel)] = viewModel;
            }

            CurrentViewModel = viewModel;
            OnNavigated?.Invoke(viewModel);
        }
    } 
}
