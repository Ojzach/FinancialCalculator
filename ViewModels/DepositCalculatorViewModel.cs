using FinancialCalculator.Commands;
using FinancialCalculator.Models;
using FinancialCalculator.Stores;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

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
                _depositStore.FederalTaxAmt = federalTaxAmtPct.Amount;
                OnPropertyChanged(nameof(FederalTaxAmt)); 
                OnPropertyChanged(nameof(FederalTaxPct)); 
            } 
        }
        public float FederalTaxPct { 
            get => federalTaxAmtPct.Percent;
            set 
            { 
                federalTaxAmtPct.Percent = value;
                _depositStore.FederalTaxAmt = federalTaxAmtPct.Amount;
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
                _depositStore.MedicareAmt = medicareAmtPct.Amount;
                OnPropertyChanged(nameof(MedicareAmt)); 
                OnPropertyChanged(nameof(MedicarePct)); 
            }
        }
        public float MedicarePct
        {
            get => medicareAmtPct.Amount;
            set 
            {
                medicareAmtPct.Percent = value;
                _depositStore.MedicareAmt = medicareAmtPct.Amount;
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
                _depositStore.SocialSecurityAmt = socialSecurityAmtPct.Amount;
                OnPropertyChanged(nameof(SocialSecurityAmt)); 
                OnPropertyChanged(nameof(SocialSecurityPct)); 
            }
        }
        public float SocialSecurityPct
        {
            get => socialSecurityAmtPct.Amount;
            set 
            { 
                socialSecurityAmtPct.Percent = value;
                _depositStore.SocialSecurityAmt = socialSecurityAmtPct.Amount;
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
                _depositStore.StateTaxAmt = stateTaxAmtPct.Amount;
                OnPropertyChanged(nameof(StateTaxAmt)); 
                OnPropertyChanged(nameof(StateTaxPct)); }
        }
        public float StateTaxPct
        {
            get => stateTaxAmtPct.Percent;
            set 
            { 
                stateTaxAmtPct.Percent = value;
                _depositStore.StateTaxAmt = stateTaxAmtPct.Amount;
                OnPropertyChanged(nameof(StateTaxAmt)); 
                OnPropertyChanged(nameof(StateTaxPct));
            }
        }



        private BudgetDepositViewModel depositBudgets;
        public BudgetDepositViewModel DepositBudgets { get => depositBudgets; set { depositBudgets = value; OnPropertyChanged(nameof(DepositBudgets)); } }



        private bool editPanelOpen = false;
        public Visibility EditPanelVisibility { get => editPanelOpen ? Visibility.Visible : Visibility.Collapsed; }

        private ViewModelBase? currentlyEditingBudget;
        public ViewModelBase? CurrentlyEditingBudget { get => currentlyEditingBudget; set { currentlyEditingBudget = value;  OnPropertyChanged(nameof(CurrentlyEditingBudget)); } }

        private readonly FinancialInstitutionsStore _financialInstituitonsStore;

        public DepositCalculatorViewModel(FinancialInstitutionsStore financialInstitutionsStore, BudgetStore budgetsStore)
        {
            _financialInstituitonsStore = financialInstitutionsStore;

            _depositStore = new DepositStore();

            federalTaxAmtPct = new AmountPercentModel(() => _depositStore.GetDepositAmount(true));
            medicareAmtPct = new AmountPercentModel(() => _depositStore.GetDepositAmount(true));
            socialSecurityAmtPct = new AmountPercentModel(() => _depositStore.GetDepositAmount(true));
            stateTaxAmtPct = new AmountPercentModel(() => _depositStore.GetDepositAmount(true));


            FederalTaxPct = 0.145f;
            MedicarePct = 0.0145f;
            SocialSecurityPct = 0.062f;




            DepositBudgets = new FixedBudgetDepositViewModel(budgetsStore.Budgets[1] as FixedBudget, budgetsStore, _depositStore);

            DepositBudgets.BudgetValueChanged += (BudgetDepositViewModel bVM) => UpdateCalculatedValues();
            DepositBudgets.EditBudgetAction += OpenEditMenu;
            _depositStore.DepositChanged += UpdateCalculatedValues;
            



            CloseEditMenuCommand = new RelayCommand(execute => CloseEditMenu());



        }



        public void UpdateCalculatedValues()
        {
            OnPropertyChanged("EstimatedYearlyIncome");
            OnPropertyChanged("MonthsCoveredByPaycheck");

            _depositStore.UpdateDeductions(federalTaxAmtPct.Amount, medicareAmtPct.Amount, socialSecurityAmtPct.Amount, stateTaxAmtPct.Amount);

            OnPropertyChanged(nameof(FederalTaxAmt));
            OnPropertyChanged(nameof(FederalTaxPct));
            OnPropertyChanged(nameof(MedicareAmt));
            OnPropertyChanged(nameof(MedicarePct));
            OnPropertyChanged(nameof(SocialSecurityAmt));
            OnPropertyChanged(nameof(SocialSecurityPct));
            OnPropertyChanged(nameof(StateTaxAmt));
            OnPropertyChanged(nameof(StateTaxPct));

            DepositBudgets.DepositAmt = _depositStore.TakeHomeAmount;

        }


        public ICommand CloseEditMenuCommand { get; set; }

        private bool editingItem = false;

        public void OpenEditMenu(ViewModelBase budget)
        {
            editingItem = true;
            CurrentlyEditingBudget = budget;
            editPanelOpen = true;
            OnPropertyChanged(nameof(EditPanelVisibility));
        }

        public void CloseEditMenu()
        {
            editPanelOpen = false;
            OnPropertyChanged(nameof(EditPanelVisibility));
            CurrentlyEditingBudget = null;
        }
    }

}
