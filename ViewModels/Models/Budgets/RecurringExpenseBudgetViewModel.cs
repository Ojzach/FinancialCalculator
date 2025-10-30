using FinancialCalculator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.ViewModels
{
    public class RecurringExpenseBudgetViewModel : FixedBudgetViewModel
    {

        public override string BudgetType => "Recurring Expense Budget";
        private RecurringExpenseBudget RecurringBudget => (RecurringExpenseBudget)_budget;

        public ObservableCollection<RecurringFrequencyType> FrequencyTypes { get; } = new(Enum.GetValues<RecurringFrequencyType>());

        public float ExpenseAmount
        {
            get => RecurringBudget.ExpenseAmount;
            set
            {
                if (RecurringBudget.ExpenseAmount != value)
                {
                    RecurringBudget.ExpenseAmount = value;
                    OnPropertyChanged(nameof(ExpenseAmount));
                }
            }
        }

        public int FrequencyValue
        {
            get => RecurringBudget.FrequencyValue;
            set
            {
                int sanitizedValue = Math.Max(1, value);

                if (RecurringBudget.FrequencyValue != sanitizedValue)
                {
                    RecurringBudget.FrequencyValue = sanitizedValue;
                    OnPropertyChanged(nameof(FrequencyValue));
                }
            }
        }

        public RecurringFrequencyType SelectedFrequencyType
        {
            get => RecurringBudget.FrequencyType;
            set
            {
                if (RecurringBudget.FrequencyType != value)
                {
                    RecurringBudget.FrequencyType = value;
                    OnPropertyChanged(nameof(SelectedFrequencyType));
                }
            }
        }

        public RecurringExpenseBudgetViewModel(RecurringExpenseBudget budget) : base(budget)
        {
        }


    }
}
