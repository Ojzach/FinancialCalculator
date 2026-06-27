using FinancialCalculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Models
{
    public class RecurringExpenseBudget : FixedBudget
    {

        public override string BudgetType { get => "Recurring Expense"; }

        public decimal ExpenseAmount
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

        private decimal expenseAmount = 0m;
        private int frequencyValue = 1;
        private RecurringFrequencyType frequencyType = RecurringFrequencyType.Month;

        private decimal FrequencyInMonths => frequencyType switch
        {
            RecurringFrequencyType.Day => frequencyValue / 30,
            RecurringFrequencyType.Week => frequencyValue / 4,
            RecurringFrequencyType.Month => frequencyValue,
            RecurringFrequencyType.Year => frequencyValue * 12,
            _ => frequencyValue
        };

        public RecurringExpenseBudget() { }

        public RecurringExpenseBudget(int id, string name, BudgetPriority priority, FinancialAccount associatedFinancialAccount, decimal expenseAmount = 0, int frequencyValue = 1, RecurringFrequencyType frequencyType = RecurringFrequencyType.Month, List<int>? childBudgets = null)
            : base(id, name, priority, associatedFinancialAccount, childBudgets: childBudgets)
        {
            ExpenseAmount = expenseAmount;
            FrequencyValue = frequencyValue;
            FrequencyType = frequencyType;
        }

        public override decimal MinDepositAmount(decimal referenceDeposit = 0, int numMonths = 1) 
            => RecommendedDepositAmount(referenceDeposit, numMonths);
        public override decimal MaxDepositAmount(decimal referenceDeposit = 0, int numMonths = 1) 
            => RecommendedDepositAmount(referenceDeposit, numMonths);
        public override decimal RecommendedDepositAmount(decimal referenceDeposit = 0, int numMonths = 1)
            => Math.Round(ExpenseAmount / FrequencyInMonths, 2) * numMonths;

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
