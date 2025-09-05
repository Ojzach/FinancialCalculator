using FinancialCalculator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.ViewModels
{
    public class RecurringExpenseBudgetViewModel : FixedBudgetViewModel
    {
        public RecurringExpenseBudgetViewModel(RecurringExpenseBudget budget) : base(budget)
        {
        }
    }
}
