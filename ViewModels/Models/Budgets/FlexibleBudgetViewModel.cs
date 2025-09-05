using FinancialCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.ViewModels
{
    public class FlexibleBudgetViewModel : BudgetViewModel
    {
        public FlexibleBudgetViewModel(FlexibleBudget budget) : base(budget)
        {
        }
    }
}
