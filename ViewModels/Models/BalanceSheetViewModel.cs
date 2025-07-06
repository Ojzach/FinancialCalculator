using FinancialCalculator.Stores;

namespace FinancialCalculator.ViewModels
{
    class BalanceSheetViewModel : BalanceSheetBaseViewModel
    {

        public BalanceSheetViewModel(PaycheckStore paycheck, string balanceSheetName, bool preTaxBalanceSheet = false) : base(paycheck, balanceSheetName, preTaxBalanceSheet)
        {
            
        }
    }
}
