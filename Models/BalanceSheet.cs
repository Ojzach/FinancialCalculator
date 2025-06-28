using FinancialCalculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Model
{
    internal class BalanceSheet
    {

        public string name;

        public ObservableCollection<BalanceItem> BalanceItems = new ObservableCollection<BalanceItem>();

        public float maxTotalSavingsPercent;

        public BalanceSheet() 
        {
            
        }
    }
}
