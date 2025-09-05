using FinancialCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.ViewModels
{
    public class SavingsBudgetViewModel : FlexibleBudgetViewModel
    {

        private SavingsBudget _budget;

        public SavingsBudgetViewModel(SavingsBudget budget) : base(budget)
        {
            _budget = budget;
        }
    }
}
