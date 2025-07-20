using FinancialCalculator.Commands;
using FinancialCalculator.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FinancialCalculator.ViewModels
{
    internal class FinancialInstitutionViewModel : ViewModelBase
    {

        private FinancialInstitution _financialInstitution;



        public string InstitutionName { get => _financialInstitution.Name; set { _financialInstitution.Name = value; OnPropertyChanged(nameof(InstitutionName)); } }

        private ObservableCollection<FinancialAccountViewModel> financialAccounts = new ObservableCollection<FinancialAccountViewModel>();
        public ObservableCollection<FinancialAccountViewModel> FinancialAccounts { get => financialAccounts; set { financialAccounts = value; OnPropertyChanged(nameof(FinancialAccounts)); } }
        private FinancialAccountViewModel? selectedFinancialAccount = null;
        public FinancialAccountViewModel? SelectedFinancialAccount { get => selectedFinancialAccount; set { selectedFinancialAccount = value; OnPropertyChanged("SelectedFinancialAccount"); } }

        
        public FinancialInstitutionViewModel(FinancialInstitution financialInstitution)
        {
            _financialInstitution = financialInstitution;

            AddFinancialAccount(new FinancialAccount("Spending", BankAccountType.Checking, 0));
            AddFinancialAccount(new FinancialAccount("Saving", BankAccountType.Savings, 1000.0f));
            AddFinancialAccount(new FinancialAccount("Credit Card", BankAccountType.Credit, 50.34f));

            OpenAddFinancialAccountMenuCommand = new RelayCommand(execute => OpenAddFinancialAccountMenu());
        }

        public void AddFinancialAccount(FinancialAccount account)
        {
            _financialInstitution.financialAccounts.Add(account);
            FinancialAccounts.Add(new FinancialAccountViewModel(account));
            FinancialAccounts[FinancialAccounts.Count - 1].openEditAccount += OpenEditAccount;
        }

        private void OpenEditAccount(FinancialAccountViewModel account) => openEditAccount?.Invoke(account);

        public event Action<FinancialAccountViewModel> openEditAccount;

        public event Action<FinancialInstitutionViewModel> openAddAccount;
        public ICommand OpenAddFinancialAccountMenuCommand { get; set; }
        private void OpenAddFinancialAccountMenu() => openAddAccount?.Invoke(this);


        public FinancialInstitution BaseFinancialInstitutionItem { get => _financialInstitution; }
    }
}
