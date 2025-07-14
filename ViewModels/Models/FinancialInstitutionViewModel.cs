using FinancialCalculator.Models;
using System.Collections.ObjectModel;

namespace FinancialCalculator.ViewModels
{
    internal class FinancialInstitutionViewModel : ViewModelBase
    {

        private FinancialInstitution _financialInstitution;



        public string InstitutionName { get => _financialInstitution.Name; set { _financialInstitution.Name = value; OnPropertyChanged(nameof(InstitutionName)); } }

        private ObservableCollection<FinancialAccountViewModel> financialAccounts = new ObservableCollection<FinancialAccountViewModel>();
        public ObservableCollection<FinancialAccountViewModel> FinancialAccounts { get => financialAccounts; set { financialAccounts = value; OnPropertyChanged(nameof(FinancialAccounts)); } }
    
        
        public FinancialInstitutionViewModel(FinancialInstitution financialInstitution)
        {
            _financialInstitution = financialInstitution;

            AddFinancialAccount(new FinancialAccount("Spending", BankAccountType.Checking, 0));
            AddFinancialAccount(new FinancialAccount("Saving", BankAccountType.Savings, 1000.0f));
            AddFinancialAccount(new FinancialAccount("Credit Card", BankAccountType.Credit, 50.34f));
        }

        public void AddFinancialAccount(FinancialAccount account)
        {
            FinancialAccounts.Add(new FinancialAccountViewModel(account));
            FinancialAccounts[FinancialAccounts.Count - 1].openEditAccount += OpenEditAccount;
        }

        private void OpenEditAccount(FinancialAccount account) => openEditAccount?.Invoke(account);

        public event Action<FinancialAccount> openEditAccount;
    }
}
