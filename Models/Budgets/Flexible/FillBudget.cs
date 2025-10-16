using FinancialCalculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Models
{
    class FillBudget : FlexibleBudget
    {

        public override string BudgetType { get => "Fill"; }

        public FillBudget(int id, string name, BudgetPriority priority, FinancialAccount associatedFinancialAccount) : base(id, name, priority, associatedFinancialAccount)
        {
        }

        public override float MinDepositAmount(float referenceDeposit, int numMonths = 1) => 0;
        public override float MaxDepositAmount(float referenceDeposit, int numMonths = 1) => float.MaxValue;
        public override float RecommendedDepositAmount(float referenceDeposit, int numMonths = 1) => 0;

        public override ViewModelBase ToViewModel()
        {
            throw new NotImplementedException();
        }
    }
}
