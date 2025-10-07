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

        public RecurringExpenseBudget(int id, string name, FinancialAccount associatedFinancialAccount, float _expenseAmt = 0, float freqMonths = 1) : base(id, name, associatedFinancialAccount)
        {
            ExpenseAmt = _expenseAmt;
            FreqMonths = freqMonths;
        }

        public override float MinDepositAmount(float referenceDeposit = 0, int numMonths = 1) => RecommendedDepositAmount(numMonths);
        public override float MaxDepositAmount(float referenceDeposit = 0, int numMonths = 1) => RecommendedDepositAmount(numMonths);
        public override float RecommendedDepositAmount(float referenceDeposit = 0, int numMonths = 1) => (ExpenseAmt / FreqMonths) * numMonths;

        public override ViewModelBase ToViewModel() => new RecurringExpenseBudgetViewModel(this);
    }
}
