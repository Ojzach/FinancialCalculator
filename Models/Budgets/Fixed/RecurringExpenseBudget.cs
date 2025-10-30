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

        public float ExpenseAmount
        {
            get => expenseAmount;
            set => expenseAmount = value;
        }

        public int FrequencyValue
        {
            get => frequencyValue;
            set => frequencyValue = Math.Max(1, value);
        }

        public RecurringFrequencyType FrequencyType
        {
            get => frequencyType;
            set => frequencyType = value;
        }

        private float expenseAmount = 0f;
        private int frequencyValue = 1;
        private RecurringFrequencyType frequencyType = RecurringFrequencyType.Month;

        private float FrequencyInMonths => frequencyType switch
        {
            RecurringFrequencyType.Day => frequencyValue / 30f,
            RecurringFrequencyType.Week => frequencyValue / 4f,
            RecurringFrequencyType.Month => frequencyValue,
            RecurringFrequencyType.Year => frequencyValue * 12f,
            _ => frequencyValue
        };

        public RecurringExpenseBudget(int id, string name, BudgetPriority priority, FinancialAccount associatedFinancialAccount, float expenseAmount = 0, int frequencyValue = 1, RecurringFrequencyType frequencyType = RecurringFrequencyType.Month, List<int>? childBudgets = null)
            : base(id, name, priority, associatedFinancialAccount, childBudgets: childBudgets)
        {
            ExpenseAmount = expenseAmount;
            FrequencyValue = frequencyValue;
            FrequencyType = frequencyType;
        }

        public override float MinDepositAmount(float referenceDeposit = 0, int numMonths = 1) => RecommendedDepositAmount(referenceDeposit, numMonths);
        public override float MaxDepositAmount(float referenceDeposit = 0, int numMonths = 1) => RecommendedDepositAmount(referenceDeposit, numMonths);
        public override float RecommendedDepositAmount(float referenceDeposit = 0, int numMonths = 1)
        {

            if (FrequencyInMonths <= 0)
            {
                return 0f;
            }

            return (ExpenseAmount / FrequencyInMonths) * numMonths;
        }

        public override ViewModelBase ToViewModel() => new RecurringExpenseBudgetViewModel(this);
    }

    public enum RecurringFrequencyType
    {
        Day,
        Week,
        Month,
        Year
    }
}
