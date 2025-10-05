using FinancialCalculator.Stores;
using FinancialCalculator.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VehicleMaintenanceLog.Commands;

namespace FinancialCalculator.ViewModels
{
    class NavigationPanelViewModel : ViewModelBase
    {

        private readonly NavigationStore _navigationStore;

        private DepositCalculatorViewModel _depositCalculatorView;
        private FinancialAccountsPageViewModel _financialAccountsPageViewModel;
        public ICommand LoadDepositCalculatorViewCommand { get; }
        public ICommand LoadFinancialAccountsViewCommand { get; }

        public NavigationPanelViewModel(NavigationStore navigationStore, FinancialInstitutionsStore financialInstitutionsStore, BudgetStore budgetsStore)
        {
            _navigationStore = navigationStore;

            _depositCalculatorView = new DepositCalculatorViewModel(financialInstitutionsStore, budgetsStore);
            _financialAccountsPageViewModel = new FinancialAccountsPageViewModel(financialInstitutionsStore);

            LoadDepositCalculatorViewCommand = new NavigateCommand(navigationStore, () => _depositCalculatorView);
            LoadFinancialAccountsViewCommand = new NavigateCommand(navigationStore, () => _financialAccountsPageViewModel);

            _navigationStore.CurrentViewModel = _depositCalculatorView;


            LoadDepositCalculatorViewCommand.Execute(null);
        }


    }
}
