using FinancialCalculator.Models;
using System.Collections.ObjectModel;
using NodaTime;
using FinancialCalculator.Stores;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using FinancialCalculator.Commands;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace FinancialCalculator.ViewModels
{
    internal class BudgetDepositViewModel : ViewModelBase
    {
        private ObservableCollection<BudgetDepositViewModel> subItems = new ObservableCollection<BudgetDepositViewModel>();
        public ObservableCollection<BudgetDepositViewModel> SubItems { 
            get => subItems; 
            set { 
                subItems = value; 
                OnPropertyChanged(nameof(SubItems)); 
                OnPropertyChanged(nameof(SubItemVisibility));
            } }

        protected BudgetDepositViewModel UnallocattedBudget;

        public virtual bool IsAmtEditable { get => (budget is not FillBudget && budget.Name != "Deposit"); }
        private bool _isVisible = true;
        public bool IsVisible { get => _isVisible; set { _isVisible = value; OnPropertyChanged(nameof(IsVisible)); } }
        public Visibility SubItemVisibility { get => SubItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }



        public string BudgetName { get => budget.Name; }
        public string BudgetType { get => budget.BudgetType; }


        public bool IsUsrSet { get => depositStore.BudgetDeposits[budgetID].DepositIsUserSet; set { depositStore.BudgetDeposits[budgetID].DepositIsUserSet = value; OnPropertyChanged(nameof(IsUsrSet)); } }
        public bool IsSetByAmt { get => depositAmtPct.IsSetByAmount; }

        public float UsrDepositPct { get => DepositPct; set { IsUsrSet = true; DepositPct = value; OnPropertyChanged(nameof(IsSetByAmt));} }
        public float DepositPct { 
            get => DepositAmt / depositStore.GetBudgetReferenceAmount(budgetID); 
            set {
                float startAmt = depositAmtPct.Amount;
                depositAmtPct.Percent = value;
                OnPropertyChanged(nameof(UsrDepositPct));
                OnPropertyChanged(nameof(UsrDepositAmt));
            } 
        }
        public float UsrDepositAmt { get => DepositAmt; set { IsUsrSet = true; DepositAmt = value; OnPropertyChanged(nameof(IsSetByAmt)); } }
        public float DepositAmt { 
            get => depositAmtPct.Amount + budget.ChildBudgets.Sum(childID => depositStore.GetBudgetDepositAmount(childID)); 
            set {
                float startAmt = depositAmtPct.Amount;
                depositAmtPct.Amount = value;
                OnPropertyChanged(nameof(UsrDepositPct));
                OnPropertyChanged(nameof(UsrDepositAmt));
            } 
        }


        public float MaxAmt { get => budget.MaxDepositAmount(depositStore.GetBudgetReferenceAmount(budget.ID)); }
        public float MinAmt { get => budget.MinDepositAmount(depositStore.GetBudgetReferenceAmount(budget.ID)); }


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
                SubItems.Last().EditBudgetAction += (budgetViewModel) => EditBudgetAction?.Invoke(budgetViewModel);
            }


            EditBudgetCommand = new RelayCommand(execute => EditBudgetAction?.Invoke(budget.ToViewModel()));
        }

        public Action<ViewModelBase> EditBudgetAction;


        private void OnDepositChanged(List<int> depositsChanged)
        {
            if (depositsChanged.Contains(0) || depositsChanged.Contains(budgetID)) RefreshUI();
        }

        public void RefreshUI()
        {
            OnPropertyChanged(nameof(UsrDepositPct));
            OnPropertyChanged(nameof(UsrDepositAmt));

            foreach(BudgetDepositViewModel budgetVM in SubItems) budgetVM.RefreshUI();
        }

    }
}
