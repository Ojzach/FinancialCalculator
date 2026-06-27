using FinancialCalculator.Stores;
using FinancialCalculator.Views;
using System.Windows.Input;
using VehicleMaintenanceLog.Commands;

namespace FinancialCalculator.ViewModels
{
    class NavigationPanelViewModel : ViewModelBase
    {
        private readonly NavigationStore _navigationStore;

        private DepositCalculatorViewModel _depositCalculatorView;
        private FinancialAccountsPageViewModel _financialAccountsPageViewModel;
        private BudgetManagementViewModel _budgetManagementViewModel;
        private BudgetOverviewViewModel _budgetOverviewViewModel;

        public ICommand LoadDepositCalculatorViewCommand { get; }
        public ICommand LoadFinancialAccountsViewCommand { get; }
        public ICommand LoadBudgetManagementViewCommand { get; }
        public ICommand LoadBudgetOverviewViewCommand { get; }

        public NavigationPanelViewModel(NavigationStore navigationStore, FinancialInstitutionsStore financialInstitutionsStore, BudgetStore budgetsStore, DepositStore depositStore)
        {
            _navigationStore = navigationStore;

            _budgetOverviewViewModel = new BudgetOverviewViewModel(budgetsStore, financialInstitutionsStore);
            _depositCalculatorView = new DepositCalculatorViewModel(financialInstitutionsStore, budgetsStore, depositStore, navigationStore, _budgetOverviewViewModel);
            _financialAccountsPageViewModel = new FinancialAccountsPageViewModel(financialInstitutionsStore);
            _budgetManagementViewModel = new BudgetManagementViewModel(budgetsStore, depositStore, financialInstitutionsStore);

            LoadDepositCalculatorViewCommand = new NavigateCommand(navigationStore, () => _depositCalculatorView);
            LoadFinancialAccountsViewCommand = new NavigateCommand(navigationStore, () => _financialAccountsPageViewModel);
            LoadBudgetManagementViewCommand = new NavigateCommand(navigationStore, () => _budgetManagementViewModel);
            LoadBudgetOverviewViewCommand = new NavigateCommand(navigationStore, () => _budgetOverviewViewModel);

            _navigationStore.CurrentViewModel = _depositCalculatorView;
            LoadDepositCalculatorViewCommand.Execute(null);
        }
    }
}
