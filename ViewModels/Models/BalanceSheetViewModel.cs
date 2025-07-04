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

        public BalanceSheetViewModel(PaycheckStore paycheck, string balanceSheetName, bool preTaxBalanceSheet = false) : base(paycheck, balanceSheetName, preTaxBalanceSheet)
        {
            
        }
    }
}
