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
        public decimal InitialDepositAmount
        {
            get => _depositStore.DepositAmount;
            set { _depositStore.DepositAmount = value; }
        }

        public decimal EstimatedYearlyIncome { get => _depositStore.EstimatedAnnualIncome; set { _depositStore.EstimatedAnnualIncome = value; } }
        public int MonthsCoveredByPaycheck { get => _depositStore.MonthsCoveredByDeposit; }


        private DepositStore _depositStore;
        private BudgetStore _budgetStore;


        private bool isEditPanelOpen = false;
        public bool IsEditPanelOpen { get => isEditPanelOpen; set { isEditPanelOpen = value; OnPropertyChanged(nameof(IsEditPanelOpen)); } }

        private int currentlyEditingBudgetID = -1;
        public ViewModelBase? CurrentlyEditingBudget => _budgetStore.Budgets.Keys.Contains(currentlyEditingBudgetID) ? _budgetStore.Budgets[currentlyEditingBudgetID].ToViewModel() : null;


        private List<BudgetDepositViewModel> depositDeductions = new List<BudgetDepositViewModel>();
        public List<BudgetDepositViewModel> DepositDeductions { get => depositDeductions; set { depositDeductions = value; OnPropertyChanged(nameof(DepositDeductions)); } }

        public BudgetDepositViewModel BaseDeposit { get; set; }

        public DepositCalculatorViewModel(FinancialInstitutionsStore financialInstitutionsStore, BudgetStore budgetsStore, DepositStore depositStore)
        {
            _depositStore = depositStore;
            _budgetStore = budgetsStore;
            _depositStore.DepositsChanged += OnDepositChanged;


            foreach (BudgetDeposit depositDeduction in _depositStore.DepositDeductions.Values)
            {
                depositDeductions.Add(new BudgetDepositViewModel(depositDeduction.DepositBudgetID, budgetsStore, _depositStore));
                depositDeductions[depositDeductions.Count() - 1].EditBudgetAction += OpenEditMenu;
            }

            BaseDeposit = new BudgetDepositViewModel(_depositStore.BaseDepositID, budgetsStore, _depositStore, true);
            BaseDeposit.EditBudgetAction += OpenEditMenu;
            
            CloseEditMenuCommand = new RelayCommand(execute => CloseEditMenu());


            _depositStore.DepositAmount = 15000;
        }



        public void OnDepositChanged(List<int> changedDeposits)
        {
            if(changedDeposits.Count == 0)
            {
                OnPropertyChanged("EstimatedYearlyIncome");
                OnPropertyChanged("MonthsCoveredByPaycheck");
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
