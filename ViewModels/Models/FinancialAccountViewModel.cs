using FinancialCalculator.Commands;
using FinancialCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FinancialCalculator.ViewModels
{
    public class FinancialAccountViewModel
    {
        private FinancialAccount _account;

        public string AccountName { get => _account.accountName; }
        public string AccountType { get => _account.accountType.ToString(); }
        public float AccountBalance { get => _account.currentBalance; }

        public FinancialAccountViewModel(FinancialAccount account)
        {
            _account = account;

            OpenEditAccountCommand = new RelayCommand(execute => OpenEditAccount());
        }


        public ICommand OpenEditAccountCommand { get; set; }

        private void OpenEditAccount() => openEditAccount?.Invoke(_account);

        public event Action<FinancialAccount> openEditAccount;
    }
}
