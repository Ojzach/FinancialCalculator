using FinancialCalculator.Commands;
using FinancialCalculator.Models;
using FinancialCalculator.Stores;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FinancialCalculator.ViewModels
{
    /// <summary>
    /// Wraps a Budget for display on the overview page and inline transaction recording.
    /// </summary>
    internal class BudgetOverviewItemViewModel : ViewModelBase
    {
        private readonly Budget _budget;
        private readonly BudgetStore _budgetStore;
        private readonly FinancialInstitutionsStore _accountsStore;

        public string BudgetName => _budget.Name;
        public string BudgetType => _budget.BudgetType;
        public string AccountName => _budget.AssociatedFinancialAccount?.accountName ?? "N/A";
        public string InstitutionName => _accountsStore.GetInstitutionNameForAccount(_budget.AssociatedFinancialAccount?.ID ?? -1);

        public decimal CurrentBalance
        {
            get => _budget.CurrentBudgetBalance;
            set { _budget.CurrentBudgetBalance = value; OnPropertyChanged(nameof(CurrentBalance)); }
        }

        // Inline transaction form
        private bool _isTransactionFormOpen;
        public bool IsTransactionFormOpen
        {
            get => _isTransactionFormOpen;
            set { _isTransactionFormOpen = value; OnPropertyChanged(nameof(IsTransactionFormOpen)); }
        }

        private decimal _transactionAmount;
        public decimal TransactionAmount
        {
            get => _transactionAmount;
            set { _transactionAmount = value; OnPropertyChanged(nameof(TransactionAmount)); }
        }

        private string _transactionDescription = "";
        public string TransactionDescription
        {
            get => _transactionDescription;
            set { _transactionDescription = value; OnPropertyChanged(nameof(TransactionDescription)); }
        }

        public ICommand OpenTransactionFormCommand { get; }
        public ICommand CancelTransactionCommand { get; }
        public ICommand RecordSpendingCommand { get; }

        public BudgetOverviewItemViewModel(Budget budget, BudgetStore budgetStore, FinancialInstitutionsStore accountsStore)
        {
            _budget = budget;
            _budgetStore = budgetStore;
            _accountsStore = accountsStore;

            OpenTransactionFormCommand = new RelayCommand(_ => IsTransactionFormOpen = true);
            CancelTransactionCommand = new RelayCommand(_ => { IsTransactionFormOpen = false; TransactionAmount = 0; TransactionDescription = ""; });
            RecordSpendingCommand = new RelayCommand(_ => RecordSpending(), _ => TransactionAmount > 0);
        }

        private void RecordSpending()
        {
            if (TransactionAmount <= 0) return;

            string desc = string.IsNullOrWhiteSpace(TransactionDescription) ? $"Spending from {BudgetName}" : TransactionDescription;

            // Deduct from balance
            _budgetStore.UpdateBudgetBalance(_budget.ID, -TransactionAmount);

            // Record transaction
            var transaction = new Transaction(desc, false, TransactionAmount, _budget.AssociatedFinancialAccount, _budget);
            _accountsStore.AddTransaction(transaction);

            OnPropertyChanged(nameof(CurrentBalance));
            TransactionAmount = 0;
            TransactionDescription = "";
            IsTransactionFormOpen = false;
        }
    }

    internal class BudgetOverviewViewModel : ViewModelBase
    {
        private readonly BudgetStore _budgetStore;
        private readonly FinancialInstitutionsStore _accountsStore;

        public ObservableCollection<BudgetOverviewItemViewModel> Budgets { get; } = new();

        public BudgetOverviewViewModel(BudgetStore budgetStore, FinancialInstitutionsStore accountsStore)
        {
            _budgetStore = budgetStore;
            _accountsStore = accountsStore;
        }

        public override void OpenViewModel()
        {
            Budgets.Clear();
            foreach (var budget in _budgetStore.Budgets.Values)
            {
                if (budget.ID == 0) continue; // skip base budget container
                Budgets.Add(new BudgetOverviewItemViewModel(budget, _budgetStore, _accountsStore));
            }
        }
    }
}
