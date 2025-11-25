using FinancialCalculator.Models;
using System.Collections.ObjectModel;
using FinancialCalculator.Stores;
using System.Windows.Input;
using FinancialCalculator.Commands;
using System.Diagnostics;

namespace FinancialCalculator.ViewModels
{
    internal class BudgetDepositViewModel : ViewModelBase
    {
        public string BudgetName { get => budget.Name; }
        public string BudgetType { get => budget.BudgetType; }

        public string BudgetPriority { get => budget.Priority.ToString(); }


        public bool DepositIsUsrSet { get => depositStore.GetDeposit(budgetID).DepositIsUserSet; set { depositStore.GetDeposit(budgetID).DepositIsUserSet = value; OnPropertyChanged(nameof(DepositIsUsrSet)); } }
        public bool DepositIsSetByAmt { get => depositAmtPct.IsSetByAmount; }

        public bool UnallocatedIsUsrSet { get => depositStore.GetDeposit(budgetID).UnallocatedIsUserSet; set { depositStore.GetDeposit(budgetID).UnallocatedIsUserSet = value; OnPropertyChanged(nameof(UnallocatedIsUsrSet)); } }
        public bool UnallocatedIsSetByAmt { get => unallocatedAmtPct.IsSetByAmount; }

        public float UsrDepositPct 
        { 
            get => UsrDepositAmt / depositStore.GetBudgetReferenceAmount(budgetID);
            set => UserChangedValue(percent: value);
        }

        public float UsrDepositAmt 
        { 
            get => depositAmtPct.Amount;
            set => UserChangedValue(amount: value);
        }

        private void UserChangedValue(float amount = -1, float percent = -1)
        {
            DepositIsUsrSet = true;
            depositStore.UsrUpdateDepositValue(budgetID, amount, percent);
            OnPropertyChanged(nameof(UsrDepositPct));
            OnPropertyChanged(nameof(UsrDepositAmt));
            OnPropertyChanged(nameof(DepositIsSetByAmt));
        }

        public float UsrUnallocatedPct
        {
            get => UsrUnallocatedAmt / depositStore.GetBudgetReferenceAmount(budgetID);
            set
            {
                UnallocatedIsUsrSet = true;
            }
        }

        public float UsrUnallocatedAmt
        {
            get => unallocatedAmtPct.Amount;
            set
            {
                UnallocatedIsUsrSet = true;
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

        public ICommand EditBudgetCommand { get; set; }


        private AmountPercentModel depositAmtPct { get => depositStore.GetDeposit(budgetID).DepositAmtPct; }
        private AmountPercentModel unallocatedAmtPct { get => depositStore.GetDeposit(budgetID).UnallocatedAmtPct; }
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
