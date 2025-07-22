using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Models
{
    public class Transaction
    {
        public string Name;

        public bool IsDeposit;
        public float Amount;

        public LocalDateTime DateTime;

        public Budget? Budget;
        public FinancialAccount Account;

        public Transaction(string name, bool isDeposit, float amount, LocalDateTime transactionTime, FinancialAccount account, Budget? budget)
        {
            Name = name;
            IsDeposit = isDeposit;
            Amount = amount;
            DateTime = transactionTime;
            Account = account;
            Budget = budget;

        }
    }
}
