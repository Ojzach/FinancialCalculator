using FinancialCalculator.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Model
{
    internal class SavingsBalanceItem : BalanceItem
    {

        private float currentSavingsAmount;
        private float goalSavingsAmount;
        private DateTime goalDate = DateTime.Today;

        public float CurrentSavingsAmount { get => currentSavingsAmount; set { currentSavingsAmount = value; SetMonthlyAmount(); } }
        public float GoalSavingsAmount { get => goalSavingsAmount; set { goalSavingsAmount = value; SetMonthlyAmount(); } }
        public DateTime GoalDate { get => goalDate; set { goalDate = value < DateTime.Today ? DateTime.Today : value; SetMonthlyAmount(); } }

        public SavingsBalanceItem(PaycheckStore paycheck, string name, float _monthlyAmount = -1.0f, float _monthlyPercent = -1.0f, string notes = "")
            : base(paycheck ,name, _monthlyAmount, _monthlyPercent, notes)
        {

        }


        private void SetMonthlyAmount()
        {
            float amountLeft = GoalSavingsAmount - CurrentSavingsAmount;
            float monthsTillGoal = (GoalDate.Year - DateTime.Today.Year) * 12 + GoalDate.Month - DateTime.Today.Month;
            if (monthsTillGoal == 0) monthsTillGoal = 1;

            if (amountLeft <= 0.0f) MonthlyAmount = 0;
            else MonthlyAmount = amountLeft / monthsTillGoal;
        }

    }
}
