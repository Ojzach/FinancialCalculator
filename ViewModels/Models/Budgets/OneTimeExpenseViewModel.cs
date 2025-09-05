using FinancialCalculator.Models;
using FinancialCalculator.Models.Budgets.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.ViewModels
{
    public class OneTimeExpenseViewModel : FixedBudgetViewModel
    {
        public OneTimeExpenseViewModel(OneTimeExpenseBudget budget) : base(budget)
        {
        }
    }
}
