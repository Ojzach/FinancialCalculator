using FinancialCalculator.Model;
using FinancialCalculator.Stores;
using FinancialCalculator.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.ViewModels.Models
{
    class SavingsBalanceSheetViewModel : BalanceSheetViewModel
    {


        public string MaxTotalSavingsPercentStr
        {
            get => (balanceSheet.maxTotalSavingsPercent * 100).ToString("0.00") + "%";
            set
            {
                float p = float.Parse(value.Trim(new Char[] { '%' }));
                balanceSheet.maxTotalSavingsPercent = p <= 100 ? p / 100 : 1;
                OnPropertyChanged("MaxTotalSavingsPercentStr");
            }
        }

        public SavingsBalanceSheetViewModel(PaycheckStore paycheck, string balanceSheetName) : base(paycheck, balanceSheetName)
        {

        }
    }
}
