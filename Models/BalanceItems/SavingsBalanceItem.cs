using FinancialCalculator.Models;
using FinancialCalculator.Stores;
using NodaTime;
using NodaTime.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Model
{
    internal class SavingsBalanceItem : BalanceItem
    {
        private float savingsGoalAmount;
        private LocalDate goalDate = DateTime.Now.ToLocalDateTime().Date;
        private BankAccount savingsAccount;
        private SavingsBalanceItemPriority savingsPriority;
        private float recommendedMonthlyAmount = 0;

        public float SavingsCurrentAmount { get => savingsAccount.currentBalance; set { savingsAccount.currentBalance = value; NumbersChanged.Invoke(); OnPropertyChanged("MonthlySavingsToReachGoal"); } }
        public float SavingsGoalAmount { get => savingsGoalAmount; set { savingsGoalAmount = value; NumbersChanged.Invoke(); OnPropertyChanged("MonthlySavingsToReachGoal"); } }
        public DateTime GoalDate { get => goalDate.ToDateTimeUnspecified(); set { goalDate = value < DateTime.Today ? DateTime.Today.ToLocalDateTime().Date : value.ToLocalDateTime().Date; NumbersChanged.Invoke(); OnPropertyChanged("MonthlySavingsToReachGoal"); } }
        public SavingsBalanceItemPriority SavingsPriority { get => savingsPriority; set { savingsPriority = value; NumbersChanged.Invoke(); } }
        
        private readonly ObservableCollection<SavingsBalanceItemPriority> priorityLevels = new ObservableCollection<SavingsBalanceItemPriority>() { SavingsBalanceItemPriority.None, SavingsBalanceItemPriority.Low, SavingsBalanceItemPriority.Medium, SavingsBalanceItemPriority.High };
        public ObservableCollection<SavingsBalanceItemPriority> PriorityLevels { get => priorityLevels; }
        
        public int MonthsTillGoalDate { get => (goalDate - DateTime.Now.ToLocalDateTime().Date).Months + 1;  }
        public float AmountLeftToSave { get => (savingsGoalAmount - savingsAccount.currentBalance) < 0 ? 0 : savingsGoalAmount - savingsAccount.currentBalance;  }
        public float MonthlySavingsToReachGoal { get => AmountLeftToSave / MonthsTillGoalDate; }
        public float RecommendedMonthlyAmount { get => recommendedMonthlyAmount; set { recommendedMonthlyAmount = value; OnPropertyChanged("RecommendedMonthlyAmount"); } }

        public SavingsBalanceItem(PaycheckStore paycheck, string name, BankAccount _savingsAccount, float _savingsGoalAmount = 0.0f, LocalDate _savingsGoalDate = default(LocalDate), SavingsBalanceItemPriority priority = SavingsBalanceItemPriority.Low, string notes = "")
            : base(paycheck ,name, notes: notes)
        {
            savingsGoalAmount = _savingsGoalAmount;
            savingsAccount = _savingsAccount;
            goalDate = _savingsGoalDate == default(LocalDate) ? DateTime.Now.ToLocalDateTime().Date : _savingsGoalDate;
            savingsPriority = priority;

            OnPropertyChanged("GoalDate");
        }


        public void SetRecommendedMonthly(float amt)
        {
            RecommendedMonthlyAmount = amt;
            SetAmountAndPercent(amount: amt);
        }

    }

    public enum SavingsBalanceItemPriority
    { 
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3,
    }

}
