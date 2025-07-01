using FinancialCalculator.Model;
using FinancialCalculator.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NodaTime;

namespace FinancialCalculator.Models.BalanceItems
{
    class RecurringBalanceItem : BalanceItem
    {


        public float OccuranceAmount;
        public RecurringFrequency Frequency;
        public LocalDate RecurringDate;

        RecurringBalanceItem(PaycheckStore paycheck, string name, BankAccount _savingsAccount, string notes = "") : base (paycheck, name, notes: notes)
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
