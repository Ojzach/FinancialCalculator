using FinancialCalculator.Stores;
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

        public NavigationPanelViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;

            LoadDepositCalculatorViewCommand = new NavigateCommand(navigationStore, () => new CalculatorViewModel());
            LoadFinancialAccountsViewCommand = new NavigateCommand(navigationStore, () => new FinancialAccountsPageViewModel());
        }

        public ICommand LoadDepositCalculatorViewCommand { get; }
        public ICommand LoadFinancialAccountsViewCommand { get; }
    }
}
