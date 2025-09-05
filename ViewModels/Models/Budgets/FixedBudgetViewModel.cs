using FinancialCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.ViewModels
{
    public class FixedBudgetViewModel : BudgetViewModel
    {
        public FixedBudgetViewModel(FixedBudget budget) : base(budget)
        {
        }
    }
}
