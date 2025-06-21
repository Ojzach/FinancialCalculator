using System.Collections.ObjectModel;
using FinancialCalculator.Model;
using FinancialCalculator.Stores;

namespace FinancialCalculator.ViewModel
{
    internal class BalanceSheetViewModel : ViewModelBase
    {
        private BalanceSheet balanceSheet;

        public string BalanceSheetName { get => balanceSheet.name; set { balanceSheet.name = value; OnPropertyChanged("BalanceSheetName"); } }

        public float TotalBalanceSheetAmount
        {
            get
            {
                float amount = 0;
                foreach(BalanceItem bi in balanceSheet.BalanceItems) amount += bi.MonthlyAmount;
                return amount;
            }
        }

        public float TotalBalanceSheetPercent { get => TotalBalanceSheetAmount / _paycheck.TakeHomeAmount; }

        public ObservableCollection<BalanceItem> BalanceSheetItems
        {  get => balanceSheet.BalanceItems; set { balanceSheet.BalanceItems = value; } }

        public string MaxTotalSavingsPercentStr {
            get => (balanceSheet.maxTotalSavingsPercent * 100).ToString("0.00") + "%";
            set
            {
                float p = float.Parse(value.Trim(new Char[] { '%' }));
                balanceSheet.maxTotalSavingsPercent = p <= 100 ? p / 100 : 1;
                OnPropertyChanged("MaxTotalSavingsPercentStr");
            }
        }

        private PaycheckStore _paycheck;
        public BalanceSheetViewModel(PaycheckStore paycheck, string balanceSheetName)
        {
            _paycheck = paycheck;
            balanceSheet = new BalanceSheet();
            BalanceSheetName = balanceSheetName;


        }


        public void AddBalanceSheetItem(BalanceItem item)
        {
            balanceSheet.BalanceItems.Add(item);
            balanceSheet.BalanceItems[balanceSheet.BalanceItems.Count - 1].NumbersChanged += 
                () => { OnPropertyChanged("TotalBalanceSheetAmount"); OnPropertyChanged("TotalBalanceSheetPercent"); };
        }
    }
}
