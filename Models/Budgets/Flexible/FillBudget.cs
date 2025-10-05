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

        public FillBudget(int id, string name, FinancialAccount associatedFinancialAccount) : base(id, name, associatedFinancialAccount)
        {
        }

        public override float GetMinMonthlyDepositAmt(float totalDeposit = 0) => 0;
        public override float GetMaxMonthlyDepositAmt(float totalDeposit = 0) => float.MaxValue;
        public override float GetRecommendedMonthlyDepositAmt(float totalDeposit) => 0;

        public override ViewModelBase ToViewModel()
        {
            throw new NotImplementedException();
        }
    }
}
