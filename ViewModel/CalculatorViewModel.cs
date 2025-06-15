using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using FinancialCalculator.Model;

namespace FinancialCalculator.ViewModel
{
    internal class CalculatorViewModel : ViewModelBase
    {

        private BalanceSheetViewModel balanceSheet;
        public BalanceSheetViewModel BalanceSheet { get { return balanceSheet; } set { balanceSheet = value; OnPropertyChanged("BalanceSheet"); } }

        public CalculatorViewModel()
        {

            balanceSheet = new BalanceSheetViewModel();

            
        }
    }

}
