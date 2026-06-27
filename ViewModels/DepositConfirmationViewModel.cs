using FinancialCalculator.Commands;
using FinancialCalculator.Models;
using FinancialCalculator.Stores;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FinancialCalculator.ViewModels
{
    /// <summary>One budget's allocation within an account group.</summary>
    public class BudgetAllocationItem
    {
        public string BudgetName { get; }
        public decimal Amount { get; }

        public BudgetAllocationItem(string name, decimal amount)
        {
            BudgetName = name;
            Amount = amount;
        }
    }

    /// <summary>All budget allocations destined for one bank account.</summary>
    public class AccountDepositGroup
    {
        public string InstitutionName { get; }
        public string AccountName { get; }
        public decimal TotalAmount => Items.Sum(i => i.Amount);
        public ObservableCollection<BudgetAllocationItem> Items { get; } = new();

        public AccountDepositGroup(string institutionName, string accountName)
        {
            InstitutionName = institutionName;
            AccountName = accountName;
        }
    }

    internal class DepositConfirmationViewModel : ViewModelBase
    {
        private readonly DepositStore _depositStore;
        private readonly BudgetStore _budgetStore;
        private readonly FinancialInstitutionsStore _accountsStore;
        private readonly NavigationStore _navigationStore;
        private readonly ViewModelBase _depositCalculatorVM;
        private readonly BudgetOverviewViewModel? _budgetOverviewVM;

        public decimal TotalDeposit { get; }
        public decimal TotalTakeHome { get; }
        public ObservableCollection<AccountDepositGroup> AccountGroups { get; } = new();
        public ObservableCollection<BudgetAllocationItem> Deductions { get; } = new();

        public ICommand BackCommand { get; }
        public ICommand ConfirmCommand { get; }

        public DepositConfirmationViewModel(
            DepositStore depositStore,
            BudgetStore budgetStore,
            FinancialInstitutionsStore accountsStore,
            NavigationStore navigationStore,
            ViewModelBase depositCalculatorVM,
            BudgetOverviewViewModel? budgetOverviewVM = null)
        {
            _depositStore = depositStore;
            _budgetStore = budgetStore;
            _accountsStore = accountsStore;
            _navigationStore = navigationStore;
            _depositCalculatorVM = depositCalculatorVM;
            _budgetOverviewVM = budgetOverviewVM;

            TotalDeposit = depositStore.DepositAmount;
            TotalTakeHome = depositStore.TakeHomeAmount;

            BuildGroups();

            BackCommand = new RelayCommand(_ => NavigateBack());
            ConfirmCommand = new RelayCommand(_ => ConfirmDeposit());
        }

        private void BuildGroups()
        {
            // Group non-deduction budget deposits by account (skip the base budget itself)
            var accountGroupMap = new Dictionary<int, AccountDepositGroup>();

            foreach (var kvp in _depositStore.BudgetDeposits)
            {
                int depositID = kvp.Key;
                if (depositID == _depositStore.BaseDepositID) continue;

                var budget = _budgetStore.GetBudget(depositID);
                var account = budget.AssociatedFinancialAccount;
                decimal amount = _depositStore.GetBudgetDepositAmount(depositID);

                if (amount <= 0) continue;

                if (!accountGroupMap.TryGetValue(account.ID, out var group))
                {
                    string instName = _accountsStore.GetInstitutionNameForAccount(account.ID);
                    group = new AccountDepositGroup(instName, account.accountName);
                    accountGroupMap[account.ID] = group;
                    AccountGroups.Add(group);
                }

                group.Items.Add(new BudgetAllocationItem(budget.Name, amount));
            }

            // Deductions
            foreach (var kvp in _depositStore.DepositDeductions)
            {
                var budget = _budgetStore.GetBudget(kvp.Key);
                decimal amount = _depositStore.GetBudgetDepositAmount(kvp.Key);
                if (amount > 0)
                    Deductions.Add(new BudgetAllocationItem(budget.Name, amount));
            }
        }

        private void NavigateBack()
        {
            _navigationStore.CurrentViewModel = _depositCalculatorVM;
        }

        private void ConfirmDeposit()
        {
            // Update budget balances and record transactions
            foreach (var kvp in _depositStore.BudgetDeposits)
            {
                int depositID = kvp.Key;
                if (depositID == _depositStore.BaseDepositID) continue;

                decimal amount = _depositStore.GetBudgetDepositAmount(depositID);
                if (amount <= 0) continue;

                var budget = _budgetStore.GetBudget(depositID);
                _budgetStore.UpdateBudgetBalance(depositID, amount);

                var transaction = new Transaction($"Paycheck Deposit → {budget.Name}", true, amount, budget.AssociatedFinancialAccount, budget);
                _accountsStore.AddTransaction(transaction);
            }

            // Also handle deductions
            foreach (var kvp in _depositStore.DepositDeductions)
            {
                decimal amount = _depositStore.GetBudgetDepositAmount(kvp.Key);
                if (amount <= 0) continue;

                var budget = _budgetStore.GetBudget(kvp.Key);
                _budgetStore.UpdateBudgetBalance(kvp.Key, amount);

                var transaction = new Transaction($"Paycheck Deduction → {budget.Name}", true, amount, budget.AssociatedFinancialAccount, budget);
                _accountsStore.AddTransaction(transaction);
            }

            // Reset deposit state then navigate to budget overview (or back to calculator)
            _depositStore.Reset();
            if (_budgetOverviewVM != null)
            {
                _budgetOverviewVM.OpenViewModel();
                _navigationStore.CurrentViewModel = _budgetOverviewVM;
            }
            else
            {
                _navigationStore.CurrentViewModel = _depositCalculatorVM;
            }
        }
    }
}
