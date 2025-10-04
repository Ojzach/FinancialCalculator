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

        public override string BudgetType => "Flexible Budget";

        public FlexibleBudgetViewModel(FlexibleBudget budget) : base(budget)
        {
        }
    }
}
