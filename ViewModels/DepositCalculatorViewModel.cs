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


        private AmtPct federalTaxAmtPct;
        private AmtPct medicareAmtPct;
        private AmtPct socialSecurityAmtPct;
        private AmtPct stateTaxAmtPct;

        public float FederalTaxAmt { 
            get => federalTaxAmtPct.DepositAmt;
            set 
            { 
                federalTaxAmtPct.SetAmt(value, _depositStore.DepositAmount);
                _depositStore.FederalTaxAmt = federalTaxAmtPct.DepositAmt;
                OnPropertyChanged(nameof(FederalTaxAmt)); 
                OnPropertyChanged(nameof(FederalTaxPct)); 
            } 
        }
        public float FederalTaxPct { 
            get => federalTaxAmtPct.DepositPct;
            set 
            { 
                federalTaxAmtPct.SetPct(value, _depositStore.DepositAmount);
                _depositStore.FederalTaxAmt = federalTaxAmtPct.DepositAmt;
                OnPropertyChanged(nameof(FederalTaxAmt)); 
                OnPropertyChanged(nameof(FederalTaxPct)); 
            }
        }

        public float MedicareAmt
        {
            get => medicareAmtPct.DepositAmt;
            set 
            { 
                medicareAmtPct.SetAmt(value, _depositStore.DepositAmount);
                _depositStore.MedicareAmt = medicareAmtPct.DepositAmt;
                OnPropertyChanged(nameof(MedicareAmt)); 
                OnPropertyChanged(nameof(MedicarePct)); 
            }
        }
        public float MedicarePct
        {
            get => medicareAmtPct.DepositPct;
            set 
            { 
                medicareAmtPct.SetPct(value, _depositStore.DepositAmount);
                _depositStore.MedicareAmt = medicareAmtPct.DepositAmt;
                OnPropertyChanged(nameof(MedicareAmt)); 
                OnPropertyChanged(nameof(MedicarePct)); 
            }
        }

        public float SocialSecurityAmt
        {
            get => socialSecurityAmtPct.DepositAmt;
            set 
            { 
                socialSecurityAmtPct.SetAmt(value, _depositStore.DepositAmount);
                _depositStore.SocialSecurityAmt = socialSecurityAmtPct.DepositAmt;
                OnPropertyChanged(nameof(SocialSecurityAmt)); 
                OnPropertyChanged(nameof(SocialSecurityPct)); 
            }
        }
        public float SocialSecurityPct
        {
            get => socialSecurityAmtPct.DepositPct;
            set 
            { 
                socialSecurityAmtPct.SetPct(value, _depositStore.DepositAmount);
                _depositStore.SocialSecurityAmt = socialSecurityAmtPct.DepositAmt;
                OnPropertyChanged(nameof(SocialSecurityAmt)); 
                OnPropertyChanged(nameof(SocialSecurityPct)); 
            }
        }

        public float StateTaxAmt
        {
            get => stateTaxAmtPct.DepositAmt;
            set 
            { 
                stateTaxAmtPct.SetAmt(value, _depositStore.DepositAmount);
                _depositStore.StateTaxAmt = stateTaxAmtPct.DepositAmt;
                OnPropertyChanged(nameof(StateTaxAmt)); 
                OnPropertyChanged(nameof(StateTaxPct)); }
        }
        public float StateTaxPct
        {
            get => stateTaxAmtPct.DepositPct;
            set 
            { 
                stateTaxAmtPct.SetPct(value, _depositStore.DepositAmount);
                _depositStore.StateTaxAmt = stateTaxAmtPct.DepositAmt;
                OnPropertyChanged(nameof(StateTaxAmt)); 
                OnPropertyChanged(nameof(StateTaxPct));
            }
        }



        private BudgetDepositViewModel depositBudgets;
        public BudgetDepositViewModel DepositBudgets { get => depositBudgets; set { depositBudgets = value; OnPropertyChanged(nameof(DepositBudgets)); } }


        private bool isEditPanelOpen = false;
        public bool IsEditPanelOpen { get => isEditPanelOpen; set { isEditPanelOpen = value; OnPropertyChanged(nameof(IsEditPanelOpen)); } }

        private ViewModelBase? currentlyEditingBudget;
        public ViewModelBase? CurrentlyEditingBudget { get => currentlyEditingBudget; set { currentlyEditingBudget = value;  OnPropertyChanged(nameof(CurrentlyEditingBudget)); } }

        private readonly FinancialInstitutionsStore _financialInstituitonsStore;

        public DepositCalculatorViewModel(FinancialInstitutionsStore financialInstitutionsStore, BudgetsStore budgetsStore)
        {
            _financialInstituitonsStore = financialInstitutionsStore;

            _depositStore = new DepositStore();

            federalTaxAmtPct = new AmtPct();
            medicareAmtPct = new AmtPct();
            socialSecurityAmtPct = new AmtPct();
            stateTaxAmtPct = new AmtPct();


            FederalTaxPct = 0.145f;
            MedicarePct = 0.0145f;
            SocialSecurityPct = 0.062f;




            DepositBudgets = new FixedBudgetDepositViewModel(budgetsStore.BaseBudget as FixedBudget, _depositStore);
            DepositBudgets.DepositPct = 1;

            DepositBudgets.BudgetValueChanged += (BudgetDepositViewModel bVM) => UpdateCalculatedValues();
            DepositBudgets.EditBudgetAction += OpenEditMenu;
            _depositStore.DepositChanged += UpdateCalculatedValues;
            



            CloseEditMenuCommand = new RelayCommand(execute => CloseEditMenu());



        }



        public void UpdateCalculatedValues()
        {
            OnPropertyChanged("EstimatedYearlyIncome");
            OnPropertyChanged("MonthsCoveredByPaycheck");

            federalTaxAmtPct.UpdateSumAmt(PaycheckAmount);
            medicareAmtPct.UpdateSumAmt(PaycheckAmount);
            socialSecurityAmtPct.UpdateSumAmt(PaycheckAmount);
            stateTaxAmtPct.UpdateSumAmt(PaycheckAmount);

            _depositStore.UpdateDeductions(federalTaxAmtPct.DepositAmt, medicareAmtPct.DepositAmt, socialSecurityAmtPct.DepositAmt, stateTaxAmtPct.DepositAmt);

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
            IsEditPanelOpen = true;
        }

        public void CloseEditMenu()
        {
            IsEditPanelOpen = false;
            if (CurrentlyEditingBudget is BudgetViewModel) DepositBudgets.ValueChanged();
            CurrentlyEditingBudget = null;
        }
    }

}
