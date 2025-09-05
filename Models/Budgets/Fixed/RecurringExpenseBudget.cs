using FinancialCalculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Models
{
    public class RecurringExpenseBudget : FixedBudget
    {

        public override string BudgetType { get => "Recurring Expense"; }

        public float ExpenseAmt = 0f;
        public float FreqMonths = 1f;

        public RecurringExpenseBudget(string name, FinancialAccount associatedFinancialAccount, float _expenseAmt = 0, float freqMonths = 1) : base(name, associatedFinancialAccount)
        {
            ExpenseAmt = _expenseAmt;
            FreqMonths = freqMonths;
        }

        public override float GetMinMonthlyDepositAmt(float totalDeposit = 0) => GetRecommendedMonthlyDepositAmt();
        public override float GetMaxMonthlyDepositAmt(float totalDeposit = 0) => GetRecommendedMonthlyDepositAmt();
        public override float GetRecommendedMonthlyDepositAmt(float totalDeposit = 0) => ExpenseAmt / FreqMonths;

        public override ViewModelBase ToViewModel()
        {
            throw new NotImplementedException();
        }
    }
}
