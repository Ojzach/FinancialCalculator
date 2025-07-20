using FinancialCalculator.Commands;
using FinancialCalculator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinancialCalculator.ViewModels
{
    public class FinancialAccountViewModel : ViewModelBase
    {
        private FinancialAccount _account;

        public string AccountName { get => _account.accountName; set { _account.accountName = value; OnPropertyChanged("AccountName"); } }
        public BankAccountType AccountType { get => _account.accountType; set { _account.accountType = value; OnPropertyChanged("AccountType"); } }
        public float AccountBalance { get => _account.currentBalance; set { _account.currentBalance = value; OnPropertyChanged("AccountBalance"); } }

        public FinancialAccountViewModel(FinancialAccount account)
        {
            _account = account;

            OpenEditAccountCommand = new RelayCommand(execute => OpenEditAccount());
        }


        public ICommand OpenEditAccountCommand { get; set; }

        private void OpenEditAccount() => openEditAccount?.Invoke(this);

        public event Action<FinancialAccountViewModel> openEditAccount;


        private ObservableCollection<BankAccountType> financialAccountTypes = new ObservableCollection<BankAccountType>() 
        { BankAccountType.Checking, BankAccountType.Savings, BankAccountType.Credit };
        public ObservableCollection<BankAccountType> FinancialAccountTypes { get => financialAccountTypes; }
    }
}
