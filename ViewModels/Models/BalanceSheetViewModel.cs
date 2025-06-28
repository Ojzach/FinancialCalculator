using FinancialCalculator.Model;
using FinancialCalculator.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.ViewModels
{
    class BalanceSheetViewModel : BalanceSheetBaseViewModel
    {

        public BalanceSheetViewModel(PaycheckStore paycheck, string balanceSheetName) : base(paycheck, balanceSheetName)
        {
        }

        protected override void BalanceItemChanged()
        {
            OnPropertyChanged("TotalBalanceSheetAmount");
            OnPropertyChanged("TotalBalanceSheetPercent");
        }
    }
}
