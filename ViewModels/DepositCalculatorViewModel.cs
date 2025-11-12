using FinancialCalculator.Commands;
using FinancialCalculator.Models;
using FinancialCalculator.Stores;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using FinancialCalculator.Services;

namespace FinancialCalculator.ViewModels
{
    internal class DepositCalculatorViewModel : ViewModelBase
    {
        public float PaycheckAmount
        {
            get => _depositStore.DepositAmount;
            set { _depositStore.DepositAmount = value; }
        }

        public float EstimatedYearlyIncome { get => _depositStore.EstimatedAnnualIncome; set { _depositStore.EstimatedAnnualIncome = value; } }
        public int MonthsCoveredByPaycheck { get => _depositStore.MonthsCoveredByDeposit; }


        private DepositStore _depositStore;
        private BudgetStore _budgetStore;



        public float TakeHomeAmount => _depositStore.TakeHomeAmount;
        public float TakeHomePercent => _depositStore.TakeHomeAmount / _depositStore.DepositAmount;


        private bool isEditPanelOpen = false;
        public bool IsEditPanelOpen { get => isEditPanelOpen; set { isEditPanelOpen = value; OnPropertyChanged(nameof(IsEditPanelOpen)); } }

        private int currentlyEditingBudgetID = -1;
        public ViewModelBase? CurrentlyEditingBudget => _budgetStore.Budgets.Keys.Contains(currentlyEditingBudgetID) ? _budgetStore.Budgets[currentlyEditingBudgetID].ToViewModel() : null;


        private List<BudgetDepositViewModel> depositBudgets = new List<BudgetDepositViewModel>();
        public List<BudgetDepositViewModel> DepositBudgets { get => depositBudgets; set { depositBudgets = value; OnPropertyChanged(nameof(DepositBudgets)); } }

        private List<BudgetDepositViewModel> depositDeductions = new List<BudgetDepositViewModel>();
        public List<BudgetDepositViewModel> DepositDeductions { get => depositDeductions; set { depositDeductions = value; OnPropertyChanged(nameof(DepositDeductions)); } }

        public DepositCalculatorViewModel(FinancialInstitutionsStore financialInstitutionsStore, BudgetStore budgetsStore)
        {
            _depositStore = new DepositStore(budgetsStore);
            _budgetStore = budgetsStore;

            _depositStore.DepositsChanged += OnDepositChanged;

            _depositStore.DepositAmount = 15000;


            foreach(BudgetDeposit depositDeduction in _depositStore.DepositDeductions.Values)
            {
                depositDeductions.Add(new BudgetDepositViewModel(depositDeduction.DepositBudgetID, budgetsStore, _depositStore));
                depositDeductions[depositDeductions.Count() - 1].EditBudgetAction += OpenEditMenu;
            }


            foreach(BudgetDeposit depositBudget in _depositStore.BudgetDeposits.Values.Where(deposit => deposit.DepositParentID == -1))
            {
                depositBudgets.Add(new BudgetDepositViewModel(depositBudget.DepositBudgetID, budgetsStore, _depositStore));
                depositBudgets[depositBudgets.Count() - 1].EditBudgetAction += OpenEditMenu;
            }
            
            CloseEditMenuCommand = new RelayCommand(execute => CloseEditMenu());
        }



        public void OnDepositChanged(List<int> changedDeposits)
        {
            if(changedDeposits.Contains(0))
            {
                OnPropertyChanged("EstimatedYearlyIncome");
                OnPropertyChanged("MonthsCoveredByPaycheck");
                OnPropertyChanged(nameof(TakeHomeAmount));
                OnPropertyChanged(nameof(TakeHomePercent));
            }
        }


        public ICommand CloseEditMenuCommand { get; set; }

        public void OpenEditMenu(int budgetID)
        {
            currentlyEditingBudgetID = budgetID;
            OnPropertyChanged(nameof(CurrentlyEditingBudget));
            IsEditPanelOpen = true;
        }

        public void CloseEditMenu()
        {
            _depositStore.UpdatedBudgetSettings(currentlyEditingBudgetID);
            IsEditPanelOpen = false;
            currentlyEditingBudgetID = -1;
        }
    }

}
