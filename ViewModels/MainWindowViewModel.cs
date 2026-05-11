
using FinancialCalculator.Stores;

namespace FinancialCalculator.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {

        private NavigationStore _navigationStore;
        private FinancialInstitutionsStore _financialInstitutionsStore;
        private ViewModelBase _currentNavigationPanelViewModel;

        public ViewModelBase CurrentViewModel => _navigationStore.CurrentViewModel;
        public ViewModelBase CurrentNavigationPanelViewModel => _currentNavigationPanelViewModel;


        public MainWindowViewModel(NavigationStore navigationStore, FinancialInstitutionsStore financialInstitutionsStore, BudgetStore budgetsStore, DepositStore depositStore) 
        { 
            _navigationStore = navigationStore;
            _financialInstitutionsStore = financialInstitutionsStore;
            _currentNavigationPanelViewModel = new NavigationPanelViewModel(navigationStore, financialInstitutionsStore, budgetsStore, depositStore);

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
