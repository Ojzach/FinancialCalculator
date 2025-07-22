using FinancialCalculator.Models;
using FinancialCalculator.Stores;
using NodaTime;
using NodaTime.Extensions;
using System.Collections.ObjectModel;

namespace FinancialCalculator.Model
{
    internal class SavingsBalanceItem : BalanceItem
    {
        private float savingsGoalAmount;
        private LocalDate goalDate = DateTime.Now.ToLocalDateTime().Date;
        private SavingsBalanceItemPriority savingsPriority;
        private float recommendedMonthlyAmount = 0;

        public float SavingsCurrentAmount { get => _bankAccount.currentBalance; set { _bankAccount.currentBalance = value; NumbersChanged.Invoke(); OnPropertyChanged("MonthlySavingsToReachGoal"); } }
        public float SavingsGoalAmount { get => savingsGoalAmount; set { savingsGoalAmount = value; NumbersChanged.Invoke(); OnPropertyChanged("MonthlySavingsToReachGoal"); } }
        public DateTime GoalDate { get => goalDate.ToDateTimeUnspecified(); set { goalDate = value < DateTime.Today ? DateTime.Today.ToLocalDateTime().Date : value.ToLocalDateTime().Date; NumbersChanged.Invoke(); OnPropertyChanged("MonthlySavingsToReachGoal"); } }
        public SavingsBalanceItemPriority SavingsPriority { get => savingsPriority; set { savingsPriority = value; NumbersChanged.Invoke(); } }
        
        private readonly ObservableCollection<SavingsBalanceItemPriority> priorityLevels = new ObservableCollection<SavingsBalanceItemPriority>() { SavingsBalanceItemPriority.None, SavingsBalanceItemPriority.Low, SavingsBalanceItemPriority.Medium, SavingsBalanceItemPriority.High };
        public ObservableCollection<SavingsBalanceItemPriority> PriorityLevels { get => priorityLevels; }
        
        public int MonthsTillGoalDate
        {
            get
            {
                Period timeSpan = (goalDate - DateTime.Now.ToLocalDateTime().Date);
                return (timeSpan.Months + timeSpan.Years * 12) + 1;
            }
        }

        public float AmountLeftToSave { get => (savingsGoalAmount - _bankAccount.currentBalance) < 0 ? 0 : savingsGoalAmount - _bankAccount.currentBalance;  }
        public float MonthlySavingsToReachGoal { get => AmountLeftToSave / MonthsTillGoalDate; }
        public float RecommendedMonthlyAmount { get => recommendedMonthlyAmount; set { recommendedMonthlyAmount = value; OnPropertyChanged("RecommendedMonthlyAmount"); } }

        public SavingsBalanceItem(PaycheckStore paycheck, string name, FinancialAccount bankAccount, float _savingsGoalAmount = 0.0f, LocalDate _savingsGoalDate = default(LocalDate), SavingsBalanceItemPriority priority = SavingsBalanceItemPriority.Low, string notes = "")
            : base(paycheck, bankAccount ,name)
        {
            savingsGoalAmount = _savingsGoalAmount;
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

    public enum SavingsBalanceItemPriority //Numbers are Set for the ratios within a tier
    { 
        //Tier 0
        None = 1,

        //Tier 1
        Low = 2,
        Medium = 4,

        //Tier 2
        High = 5,
    }

}
