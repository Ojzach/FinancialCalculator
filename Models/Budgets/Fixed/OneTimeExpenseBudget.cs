using FinancialCalculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Models.Budgets.Fixed
{
    public class OneTimeExpenseBudget : FixedBudget
    {
        public override string BudgetType { get => "Recurring Expense"; }

        public float ExpenseAmt = 0f;

        public OneTimeExpenseBudget(string name, FinancialAccount associatedFinancialAccount, float _expenseAmt = 0) : base(name, associatedFinancialAccount)
        {
            ExpenseAmt = _expenseAmt;
        }

        public override float GetMinMonthlyDepositAmt(float totalDeposit = 0) => GetRecommendedMonthlyDepositAmt();
        public override float GetMaxMonthlyDepositAmt(float totalDeposit = 0) => GetRecommendedMonthlyDepositAmt();
        public override float GetRecommendedMonthlyDepositAmt(float totalDeposit = 0) => ExpenseAmt;

        public override ViewModelBase ToViewModel()
        {
            throw new NotImplementedException();
        }
    }
}
