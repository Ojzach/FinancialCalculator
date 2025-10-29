using FinancialCalculator.Models;
using System.Collections.ObjectModel;
using FinancialCalculator.Stores;
using System.Windows.Input;
using FinancialCalculator.Commands;

namespace FinancialCalculator.ViewModels
{
    internal class BudgetDepositViewModel : ViewModelBase
    {
        public string BudgetName { get => budget.Name; }
        public string BudgetType { get => budget.BudgetType; }

        public string BudgetPriority { get => budget.Priority.ToString(); }


        public bool IsUsrSet { get => depositStore.BudgetDeposits[budgetID].DepositIsUserSet; set { depositStore.BudgetDeposits[budgetID].DepositIsUserSet = value; OnPropertyChanged(nameof(IsUsrSet)); } }
        public bool IsSetByAmt { get => depositAmtPct.IsSetByAmount; }

        public float UsrDepositPct 
        { 
            get => UsrDepositAmt / depositStore.GetBudgetReferenceAmount(budgetID);
            set { 
                IsUsrSet = true;
                depositAmtPct.Percent = value;
                OnPropertyChanged(nameof(UsrDepositPct));
                OnPropertyChanged(nameof(UsrDepositAmt));
                OnPropertyChanged(nameof(IsSetByAmt));} 
        }

        public float UsrDepositAmt 
        { 
            get => depositAmtPct.Amount; 
            set 
            { 
                IsUsrSet = true;
                depositAmtPct.Amount = value;
                OnPropertyChanged(nameof(UsrDepositPct));
                OnPropertyChanged(nameof(UsrDepositAmt)); 
                OnPropertyChanged(nameof(IsSetByAmt)); 
            } 
        }

        public ObservableCollection<BudgetDepositViewModel> SubItems
        {
            get => subItems;
            set
            {
                subItems = value;
                OnPropertyChanged(nameof(SubItems));
                OnPropertyChanged(nameof(IsSubItemsNotEmpty));
            }
        }
        private ObservableCollection<BudgetDepositViewModel> subItems = new ObservableCollection<BudgetDepositViewModel>();
        public bool IsSubItemsNotEmpty => SubItems.Count > 0;

        public bool IsDepositAmountInvalid => depositStore.BudgetDeposits[budgetID].IsDepositAmountInvalid;
        public string DepositInvalidMsg => depositStore.BudgetDeposits[budgetID].DepositInvalidMsg;
        public virtual bool IsAmtEditable { get => (budget is not FillBudget && budget.Name != "Deposit"); }


        public ICommand EditBudgetCommand { get; set; }


        private AmountPercentModel depositAmtPct { get => depositStore.BudgetDeposits[budgetID].DepositAmtPct; }
        protected Budget budget;
        private int budgetID;
        protected DepositStore depositStore;

        public BudgetDepositViewModel(int _budgetID, BudgetStore _budgetStore, DepositStore _depositStore)
        {
            budgetID = _budgetID;
            budget = _budgetStore.GetBudget(_budgetID);
            depositStore = _depositStore;

            depositStore.DepositsChanged += OnDepositChanged;

            foreach (int budgetID in budget.ChildBudgets)
            {
                SubItems.Add(new BudgetDepositViewModel(budgetID, _budgetStore, _depositStore));
                SubItems.Last().EditBudgetAction += (id) => EditBudgetAction?.Invoke(id);
            }


            EditBudgetCommand = new RelayCommand(execute => EditBudgetAction?.Invoke(budgetID));
        }

        public Action<int> EditBudgetAction;


        private void OnDepositChanged(List<int> depositsChanged)
        {
            if (depositsChanged.Contains(0) || depositsChanged.Contains(budgetID)) RefreshUI();
        }

        public void RefreshUI()
        {
            OnPropertyChanged(nameof(UsrDepositPct));
            OnPropertyChanged(nameof(UsrDepositAmt));
            OnPropertyChanged(nameof(IsDepositAmountInvalid));
            OnPropertyChanged(nameof(BudgetPriority));

            foreach (BudgetDepositViewModel budgetVM in SubItems) budgetVM.RefreshUI();
        }

    }
}
