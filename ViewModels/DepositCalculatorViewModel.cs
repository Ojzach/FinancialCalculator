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
            set { _depositStore.DepositAmount = value; UpdateCalculatedValues(); }
        }

        public float EstimatedYearlyIncome { get => _depositStore.EstimatedyearlyIncome; set { _depositStore.EstimatedyearlyIncome = value; UpdateCalculatedValues(); } }
        public int MonthsCoveredByPaycheck { get => _depositStore.MonthsCoveredByDeposit; }


        private DepositStore _depositStore;


        private AmountPercentModel federalTaxAmtPct;
        private AmountPercentModel medicareAmtPct;
        private AmountPercentModel socialSecurityAmtPct;
        private AmountPercentModel stateTaxAmtPct;

        public float FederalTaxAmt { 
            get => federalTaxAmtPct.Amount;
            set 
            {
                federalTaxAmtPct.Amount = value;
                //_depositStore.SetDepositDeductionAmt(0, federalTaxAmtPct.GetAmount(_depositStore.DepositAmount));
                OnPropertyChanged(nameof(FederalTaxAmt)); 
                OnPropertyChanged(nameof(FederalTaxPct)); 
            } 
        }
        public float FederalTaxPct { 
            get => federalTaxAmtPct.Percent;
            set 
            { 
                federalTaxAmtPct.Percent = value;
                //_depositStore.SetDepositDeductionAmt(0, federalTaxAmtPct.GetAmount(_depositStore.DepositAmount));
                OnPropertyChanged(nameof(FederalTaxAmt)); 
                OnPropertyChanged(nameof(FederalTaxPct)); 
            }
        }

        public float MedicareAmt
        {
            get => medicareAmtPct.Amount;
            set 
            {
                medicareAmtPct.Amount = value;
                //_depositStore.SetDepositDeductionAmt(1, medicareAmtPct.GetAmount(_depositStore.DepositAmount));
                OnPropertyChanged(nameof(MedicareAmt)); 
                OnPropertyChanged(nameof(MedicarePct)); 
            }
        }
        public float MedicarePct
        {
            get => medicareAmtPct.Percent;
            set 
            {
                medicareAmtPct.Percent = value;
                //_depositStore.SetDepositDeductionAmt(1, medicareAmtPct.GetAmount(_depositStore.DepositAmount));
                OnPropertyChanged(nameof(MedicareAmt)); 
                OnPropertyChanged(nameof(MedicarePct)); 
            }
        }

        public float SocialSecurityAmt
        {
            get => socialSecurityAmtPct.Amount;
            set 
            { 
                socialSecurityAmtPct.Amount = value;
                //_depositStore.SetDepositDeductionAmt(2, socialSecurityAmtPct.GetAmount(_depositStore.DepositAmount));
                OnPropertyChanged(nameof(SocialSecurityAmt)); 
                OnPropertyChanged(nameof(SocialSecurityPct)); 
            }
        }
        public float SocialSecurityPct
        {
            get => socialSecurityAmtPct.Percent;
            set 
            { 
                socialSecurityAmtPct.Percent = value;
                //_depositStore.SetDepositDeductionAmt(2, socialSecurityAmtPct.GetAmount(_depositStore.DepositAmount));
                OnPropertyChanged(nameof(SocialSecurityAmt)); 
                OnPropertyChanged(nameof(SocialSecurityPct)); 
            }
        }

        public float StateTaxAmt
        {
            get => stateTaxAmtPct.Amount;
            set 
            { 
                stateTaxAmtPct.Amount = value;
                //_depositStore.SetDepositDeductionAmt(3, socialSecurityAmtPct.GetAmount(_depositStore.DepositAmount));
                OnPropertyChanged(nameof(StateTaxAmt)); 
                OnPropertyChanged(nameof(StateTaxPct)); }
        }
        public float StateTaxPct
        {
            get => stateTaxAmtPct.Percent;
            set 
            { 
                stateTaxAmtPct.Percent = value;
                //_depositStore.SetDepositDeductionAmt(3, socialSecurityAmtPct.GetAmount(_depositStore.DepositAmount));
                OnPropertyChanged(nameof(StateTaxAmt)); 
                OnPropertyChanged(nameof(StateTaxPct));
            }
        }


        public float TakeHomeAmount => _depositStore.TakeHomeAmount;
        public float TakeHomePercent => _depositStore.TakeHomeAmount / _depositStore.DepositAmount;


        private bool isEditPanelOpen = false;
        public bool IsEditPanelOpen { get => isEditPanelOpen; set { isEditPanelOpen = value; OnPropertyChanged(nameof(IsEditPanelOpen)); } }

        private ViewModelBase? currentlyEditingBudget;
        public ViewModelBase? CurrentlyEditingBudget { get => currentlyEditingBudget; set { currentlyEditingBudget = value;  OnPropertyChanged(nameof(CurrentlyEditingBudget)); } }


        private List<BudgetDepositViewModel> depositBudgets = new List<BudgetDepositViewModel>();
        public List<BudgetDepositViewModel> DepositBudgets { get => depositBudgets; set { depositBudgets = value; OnPropertyChanged(nameof(DepositBudgets)); } }

        public DepositCalculatorViewModel(FinancialInstitutionsStore financialInstitutionsStore, BudgetStore budgetsStore)
        {
            _depositStore = new DepositStore(budgetsStore);
            _depositStore.DepositAmount = 15000;


            federalTaxAmtPct = new AmountPercentModel(() => _depositStore.DepositAmount, initialPercent: 0.145f);
            medicareAmtPct = new AmountPercentModel(() => _depositStore.DepositAmount, initialPercent: 0.0145f);
            socialSecurityAmtPct = new AmountPercentModel(() => _depositStore.DepositAmount, initialPercent: 0.062f);
            stateTaxAmtPct = new AmountPercentModel(() => _depositStore.DepositAmount, initialPercent: 0.0f);


            foreach(BudgetDeposit depositBudget in _depositStore.BudgetDeposits.Values.Where(deposit => deposit.DepositParentID == -1))
            {
                depositBudgets.Add(new BudgetDepositViewModel(depositBudget.DepositBudgetID, budgetsStore, _depositStore));
                depositBudgets[depositBudgets.Count() - 1].EditBudgetAction += OpenEditMenu;
            }
            
            CloseEditMenuCommand = new RelayCommand(execute => CloseEditMenu());
        }



        public void UpdateCalculatedValues()
        {
            OnPropertyChanged("EstimatedYearlyIncome");
            OnPropertyChanged("MonthsCoveredByPaycheck");
            OnPropertyChanged(nameof(TakeHomeAmount));
            OnPropertyChanged(nameof(TakeHomePercent));

            OnPropertyChanged(nameof(FederalTaxAmt));
            OnPropertyChanged(nameof(FederalTaxPct));
            OnPropertyChanged(nameof(MedicareAmt));
            OnPropertyChanged(nameof(MedicarePct));
            OnPropertyChanged(nameof(SocialSecurityAmt));
            OnPropertyChanged(nameof(SocialSecurityPct));
            OnPropertyChanged(nameof(StateTaxAmt));
            OnPropertyChanged(nameof(StateTaxPct));

        }


        public ICommand CloseEditMenuCommand { get; set; }

        private bool editingItem = false;
        public void OpenEditMenu(ViewModelBase budget)
        {
            editingItem = true;
            CurrentlyEditingBudget = budget;
            IsEditPanelOpen = true;
        }

        public void CloseEditMenu()
        {
            IsEditPanelOpen = false;
            CurrentlyEditingBudget = null;
        }
    }

}
