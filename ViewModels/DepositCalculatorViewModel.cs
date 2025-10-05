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
            get => federalTaxAmtPct.GetAmount(_depositStore.DepositAmount);
            set 
            {
                federalTaxAmtPct.Amount = value;
                _depositStore.FederalTaxAmt = federalTaxAmtPct.GetAmount(_depositStore.DepositAmount);
                OnPropertyChanged(nameof(FederalTaxAmt)); 
                OnPropertyChanged(nameof(FederalTaxPct)); 
            } 
        }
        public float FederalTaxPct { 
            get => federalTaxAmtPct.GetPercent(_depositStore.DepositAmount);
            set 
            { 
                federalTaxAmtPct.Percent = value;
                _depositStore.FederalTaxAmt = federalTaxAmtPct.GetAmount(_depositStore.DepositAmount);
                OnPropertyChanged(nameof(FederalTaxAmt)); 
                OnPropertyChanged(nameof(FederalTaxPct)); 
            }
        }

        public float MedicareAmt
        {
            get => medicareAmtPct.GetAmount(_depositStore.DepositAmount);
            set 
            {
                medicareAmtPct.Amount = value;
                _depositStore.MedicareAmt = medicareAmtPct.GetAmount(_depositStore.DepositAmount);
                OnPropertyChanged(nameof(MedicareAmt)); 
                OnPropertyChanged(nameof(MedicarePct)); 
            }
        }
        public float MedicarePct
        {
            get => medicareAmtPct.GetPercent(_depositStore.DepositAmount);
            set 
            {
                medicareAmtPct.Percent = value;
                _depositStore.MedicareAmt = medicareAmtPct.GetAmount(_depositStore.DepositAmount);
                OnPropertyChanged(nameof(MedicareAmt)); 
                OnPropertyChanged(nameof(MedicarePct)); 
            }
        }

        public float SocialSecurityAmt
        {
            get => socialSecurityAmtPct.GetAmount(_depositStore.DepositAmount);
            set 
            { 
                socialSecurityAmtPct.Amount = value;
                _depositStore.SocialSecurityAmt = socialSecurityAmtPct.GetAmount(_depositStore.DepositAmount);
                OnPropertyChanged(nameof(SocialSecurityAmt)); 
                OnPropertyChanged(nameof(SocialSecurityPct)); 
            }
        }
        public float SocialSecurityPct
        {
            get => socialSecurityAmtPct.GetPercent(_depositStore.DepositAmount);
            set 
            { 
                socialSecurityAmtPct.Percent = value;
                _depositStore.SocialSecurityAmt = socialSecurityAmtPct.GetAmount(_depositStore.DepositAmount);
                OnPropertyChanged(nameof(SocialSecurityAmt)); 
                OnPropertyChanged(nameof(SocialSecurityPct)); 
            }
        }

        public float StateTaxAmt
        {
            get => stateTaxAmtPct.GetAmount(_depositStore.DepositAmount);
            set 
            { 
                stateTaxAmtPct.Amount = value;
                _depositStore.StateTaxAmt = stateTaxAmtPct.GetAmount(_depositStore.DepositAmount);
                OnPropertyChanged(nameof(StateTaxAmt)); 
                OnPropertyChanged(nameof(StateTaxPct)); }
        }
        public float StateTaxPct
        {
            get => stateTaxAmtPct.GetPercent(_depositStore.DepositAmount);
            set 
            { 
                stateTaxAmtPct.Percent = value;
                _depositStore.StateTaxAmt = stateTaxAmtPct.GetAmount(_depositStore.DepositAmount);
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

            _depositStore = new DepositStore(budgetsStore);

            federalTaxAmtPct = new AmountPercentModel();
            medicareAmtPct = new AmountPercentModel();
            socialSecurityAmtPct = new AmountPercentModel();
            stateTaxAmtPct = new AmountPercentModel();


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

            _depositStore.UpdateDeductions(federalTaxAmtPct.GetAmount(_depositStore.DepositAmount), medicareAmtPct.GetAmount(_depositStore.DepositAmount), socialSecurityAmtPct.GetAmount(_depositStore.DepositAmount), stateTaxAmtPct.GetAmount(_depositStore.DepositAmount));

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
