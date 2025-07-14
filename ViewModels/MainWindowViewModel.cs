
using FinancialCalculator.Stores;

namespace FinancialCalculator.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {

        private NavigationStore _navigationStore;
        private ViewModelBase _currentNavigationPanelViewModel;

        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;
        public ViewModelBase CurrentNavigationPanelViewModel => _currentNavigationPanelViewModel;


        public MainWindowViewModel(NavigationStore navigationStore) 
        { 
            _navigationStore = navigationStore;
            _currentNavigationPanelViewModel = new NavigationPanelViewModel(navigationStore);

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
