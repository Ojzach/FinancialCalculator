using FinancialCalculator.Commands;
using FinancialCalculator.Models;
using FinancialCalculator.Models.Budgets.Fixed;
using FinancialCalculator.Stores;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FinancialCalculator.ViewModels
{
    internal class BudgetManagementViewModel : ViewModelBase
    {
        private readonly BudgetStore _budgetStore;
        private readonly DepositStore _depositStore;
        private readonly FinancialInstitutionsStore _accountsStore;

        // --- Budget list ---
        public ObservableCollection<BudgetViewModel> Budgets { get; } = new();

        // --- Edit panel ---
        private bool _isEditPanelOpen;
        public bool IsEditPanelOpen
        {
            get => _isEditPanelOpen;
            set { _isEditPanelOpen = value; OnPropertyChanged(nameof(IsEditPanelOpen)); }
        }

        private int _editingBudgetID = -1;
        private ViewModelBase? _currentlyEditingBudgetVM;
        public ViewModelBase? CurrentlyEditingBudget => _currentlyEditingBudgetVM;

        // --- Add form ---
        private bool _isAddFormOpen;
        public bool IsAddFormOpen
        {
            get => _isAddFormOpen;
            set { _isAddFormOpen = value; OnPropertyChanged(nameof(IsAddFormOpen)); }
        }

        private string _newBudgetName = "New Budget";
        public string NewBudgetName
        {
            get => _newBudgetName;
            set { _newBudgetName = value; OnPropertyChanged(nameof(NewBudgetName)); }
        }

        public ObservableCollection<string> BudgetTypes { get; } = new()
        {
            "Fixed", "Flexible", "Recurring Expense", "Savings Goal", "One-Time Expense"
        };

        private string _selectedBudgetType = "Fixed";
        public string SelectedBudgetType
        {
            get => _selectedBudgetType;
            set { _selectedBudgetType = value; OnPropertyChanged(nameof(SelectedBudgetType)); }
        }

        public ObservableCollection<FinancialAccountViewModel> AvailableAccounts { get; } = new();

        private FinancialAccountViewModel? _selectedAccount;
        public FinancialAccountViewModel? SelectedAccount
        {
            get => _selectedAccount;
            set { _selectedAccount = value; OnPropertyChanged(nameof(SelectedAccount)); }
        }

        // --- Commands ---
        public ICommand OpenAddFormCommand { get; }
        public ICommand CancelAddFormCommand { get; }
        public ICommand ConfirmAddBudgetCommand { get; }
        public ICommand EditBudgetCommand { get; }
        public ICommand DeleteBudgetCommand { get; }
        public ICommand CloseEditMenuCommand { get; }

        public BudgetManagementViewModel(BudgetStore budgetStore, DepositStore depositStore, FinancialInstitutionsStore accountsStore)
        {
            _budgetStore = budgetStore;
            _depositStore = depositStore;
            _accountsStore = accountsStore;

            OpenAddFormCommand = new RelayCommand(_ => IsAddFormOpen = true);
            CancelAddFormCommand = new RelayCommand(_ => IsAddFormOpen = false);
            ConfirmAddBudgetCommand = new RelayCommand(_ => ConfirmAddBudget());
            EditBudgetCommand = new RelayCommand(id => OpenEditMenu(Convert.ToInt32(id)));
            DeleteBudgetCommand = new RelayCommand(id => DeleteBudget(Convert.ToInt32(id)));
            CloseEditMenuCommand = new RelayCommand(_ => CloseEditMenu());
        }

        public override void OpenViewModel()
        {
            RefreshBudgetList();
            RefreshAccountList();
        }

        private void RefreshBudgetList()
        {
            Budgets.Clear();
            foreach (var budget in _budgetStore.Budgets.Values)
                Budgets.Add((BudgetViewModel)budget.ToViewModel());
        }

        private void RefreshAccountList()
        {
            AvailableAccounts.Clear();
            foreach (var account in _accountsStore.GetAllAccounts().Values)
            {
                var vm = new FinancialAccountViewModel(account);
                AvailableAccounts.Add(vm);
            }
            SelectedAccount = AvailableAccounts.FirstOrDefault();
        }

        private void OpenEditMenu(int budgetID)
        {
            _editingBudgetID = budgetID;
            _currentlyEditingBudgetVM = _budgetStore.GetBudget(budgetID).ToViewModel();
            OnPropertyChanged(nameof(CurrentlyEditingBudget));
            IsEditPanelOpen = true;
        }

        private void CloseEditMenu()
        {
            if (_editingBudgetID >= 0)
            {
                _budgetStore.SaveBudget(_editingBudgetID);
                _depositStore.UpdatedBudgetSettings(_editingBudgetID);
            }

            _currentlyEditingBudgetVM = null;
            OnPropertyChanged(nameof(CurrentlyEditingBudget));
            IsEditPanelOpen = false;
            _editingBudgetID = -1;

            RefreshBudgetList();
        }

        private void ConfirmAddBudget()
        {
            if (SelectedAccount == null) return;

            var account = _accountsStore.GetFinancialAccount(SelectedAccount.BaseAccount.ID);

            Budget newBudget = SelectedBudgetType switch
            {
                "Fixed" => new FixedBudget(0, NewBudgetName, BudgetPriority.Medium, account),
                "Flexible" => new FlexibleBudget(0, NewBudgetName, BudgetPriority.Medium, account),
                "Recurring Expense" => new RecurringExpenseBudget(0, NewBudgetName, BudgetPriority.Medium, account),
                "Savings Goal" => new SavingsBudget(0, NewBudgetName, BudgetPriority.Medium, account),
                "One-Time Expense" => new OneTimeExpenseBudget(0, NewBudgetName, BudgetPriority.Medium, account),
                _ => new FixedBudget(0, NewBudgetName, BudgetPriority.Medium, account)
            };

            // BudgetStore assigns the actual ID and saves to DB
            _budgetStore.AddBudget(newBudget);

            // Add to Base Budget's children so it participates in deposit allocation
            var baseBudget = _budgetStore.Budgets[0];
            if (!baseBudget.ChildBudgets.Contains(newBudget.ID))
            {
                baseBudget.ChildBudgets.Add(newBudget.ID);
                _budgetStore.SaveBudget(0);
            }

            // Register with DepositStore so allocation works immediately
            _depositStore.RegisterNewBudget(newBudget.ID, parentBudgetID: 0);

            IsAddFormOpen = false;
            NewBudgetName = "New Budget";

            RefreshBudgetList();
        }

        private void DeleteBudget(int budgetID)
        {
            // Remove from base budget's children
            var baseBudget = _budgetStore.Budgets[0];
            baseBudget.ChildBudgets.Remove(budgetID);
            _budgetStore.SaveBudget(0);

            _budgetStore.DeleteBudget(budgetID);
            RefreshBudgetList();
        }
    }
}
