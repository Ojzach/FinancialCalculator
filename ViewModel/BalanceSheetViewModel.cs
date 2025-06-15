using System.Collections.ObjectModel;
using FinancialCalculator.Model;

namespace FinancialCalculator.ViewModel
{
    internal class BalanceSheetViewModel : ViewModelBase
    {


        private BalanceSheet balanceSheet;

        public float PaycheckAmount { 
            get => balanceSheet.paycheckAmount; 
            set { balanceSheet.paycheckAmount = value; UpdateCalculatedValues(); } }

        public float TakeHomeAmount { get => balanceSheet.takeHomeAmount; }


        #region Taxes

        public float TotalTaxAmount { get {
                return balanceSheet is null ? 0f : balanceSheet.federalTaxAmount + balanceSheet.stateTaxAmount; } }

        public float TotalTaxPercent { get => TotalTaxAmount / PaycheckAmount; }
        public float FederalTaxPercent { get => FederalTaxAmount / PaycheckAmount; }
        public float StateTaxPercent { get => StateTaxAmount / PaycheckAmount; }

        public float FederalTaxAmount
        {
            get => balanceSheet is null ? 0f : balanceSheet.federalTaxAmount;
            set { 
                balanceSheet.federalTaxAmount = value; UpdateCalculatedValues(); }
        }

        public float StateTaxAmount
        {
            get => balanceSheet is null ? 0f : balanceSheet.stateTaxAmount;
            set { balanceSheet.stateTaxAmount = value; UpdateCalculatedValues(); }
        }


        #endregion

        public float TotalInvestmentAmount
        {
            get
            {
                float amount = 0;
                foreach(BalanceItem bi in balanceSheet.investmentBalanceItems) amount += bi.MonthlyAmount;
                return amount;
            }
        }
        public float TotalFixedCostsAmount
        {
            get
            {
                float amount = 0;
                foreach (BalanceItem bi in balanceSheet.fixedCostsBalanceItems) amount += bi.MonthlyAmount;
                return amount;
            }
        }
        public float TotalSavingsAmount
        {
            get
            {
                float amount = 0;
                foreach (BalanceItem bi in balanceSheet.savingsBalanceItems) amount += bi.MonthlyAmount;
                return amount;
            }
        }
        public float TotalFreeSpendingAmount
        {
            get
            {
                float amount = 0;
                foreach (BalanceItem bi in balanceSheet.freeSpendingBalanceItems) amount += bi.MonthlyAmount;
                return amount;
            }
        }


        public float TotalInvestmentPercent { get => TotalInvestmentAmount / TakeHomeAmount; }
        public float TotalFixedCostsPercent { get => TotalFixedCostsAmount / TakeHomeAmount; }
        public float TotalFreeSpendingPercent { get => TotalFreeSpendingAmount / TakeHomeAmount; }
        public float TotalSavingsPercent { get => TotalSavingsAmount / TakeHomeAmount; }



        public ObservableCollection<BalanceItem> InvestmentBalanceItems 
        {  get => balanceSheet.investmentBalanceItems; set { balanceSheet.investmentBalanceItems = value; UpdateCalculatedValues(); } }
        
        public ObservableCollection<BalanceItem> FixedCostsBalanceItems 
        { get => balanceSheet.fixedCostsBalanceItems; set { balanceSheet.fixedCostsBalanceItems = value; UpdateCalculatedValues(); } }

        public ObservableCollection<BalanceItem> SavingsBalanceItems
        { get => balanceSheet.savingsBalanceItems; set { balanceSheet.savingsBalanceItems = value; UpdateCalculatedValues(); } }

        public ObservableCollection<BalanceItem> FreeSpendingBalanceItems 
        { get => balanceSheet.freeSpendingBalanceItems; set { balanceSheet.freeSpendingBalanceItems = value; UpdateCalculatedValues(); } }



        public void UpdateCalculatedValues()
        {
            OnPropertyChanged("FederalTaxPercent");
            OnPropertyChanged("StateTaxPercent");
            OnPropertyChanged("TotalTaxAmount");
            OnPropertyChanged("TotalTaxPercent");
            OnPropertyChanged("TakeHomeAmount");

            OnPropertyChanged("TotalInvestmentAmount");
            OnPropertyChanged("TotalInvestmentPercent");
            OnPropertyChanged("TotalFixedCostsAmount");
            OnPropertyChanged("TotalFixedCostsPercent");
            OnPropertyChanged("TotalFreeSpendingAmount");
            OnPropertyChanged("TotalFreeSpendingPercent");

        }

        public BalanceSheetViewModel()
        {
            balanceSheet = new BalanceSheet(this);

        }
    }
}
