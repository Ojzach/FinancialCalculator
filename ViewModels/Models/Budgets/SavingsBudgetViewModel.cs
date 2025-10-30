using FinancialCalculator.Models;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.ViewModels
{
    public class SavingsBudgetViewModel : FlexibleBudgetViewModel
    {

        public override string BudgetType => "Savings Budget";

        private SavingsBudget SavingsBudget => (SavingsBudget)_budget;

        public float GoalAmount
        {
            get => SavingsBudget.SavingsGoalAmount;
            set
            {
                if (SavingsBudget.SavingsGoalAmount != value)
                {
                    SavingsBudget.SavingsGoalAmount = value;
                    OnPropertyChanged(nameof(GoalAmount));
                    OnPropertyChanged(nameof(CalculatedAmtPerMonth));
                }
            }
        }

        public DateTime? GoalDate
        {
            get => SavingsBudget.GoalDate.ToDateTimeUnspecified();
            set
            {
                if (value.HasValue)
                {
                    LocalDate updatedDate = LocalDate.FromDateTime(value.Value.Date);

                    if (SavingsBudget.GoalDate != updatedDate)
                    {
                        SavingsBudget.GoalDate = updatedDate;
                        OnPropertyChanged(nameof(GoalDate));
                        OnPropertyChanged(nameof(CalculatedAmtPerMonth));
                    }
                }
            }
        }

        public float CalculatedAmtPerMonth => SavingsBudget.RecommendedDepositAmount(0);

        public SavingsBudgetViewModel(SavingsBudget budget) : base(budget)
        {
        }
    }
}
