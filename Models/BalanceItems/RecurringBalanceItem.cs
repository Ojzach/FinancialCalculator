using FinancialCalculator.Model;
using FinancialCalculator.Stores;
using NodaTime;

namespace FinancialCalculator.Models.BalanceItems
{
    class RecurringBalanceItem : BalanceItem
    {


        public float OccuranceAmount;
        public RecurringFrequency Frequency;
        public LocalDate RecurringDate;

        RecurringBalanceItem(PaycheckStore paycheck, string name, FinancialAccount _savingsAccount, string notes = "") : base (paycheck, _savingsAccount, name, notes: notes)
        {

        }
    }

    public enum RecurringFrequency
    {
        Daily,
        Weekly,
        Monthly,
        Quarterly,
        Yearly,
    }
}
