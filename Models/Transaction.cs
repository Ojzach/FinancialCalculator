using Microsoft.EntityFrameworkCore;
using System;

namespace FinancialCalculator.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public bool IsDeposit { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; } = System.DateTime.Now;

        public int AccountId { get; set; }
        public int? BudgetId { get; set; }

        public Transaction() { }

        public Transaction(string name, bool isDeposit, decimal amount, FinancialAccount account, Budget? budget = null)
        {
            Name = name;
            IsDeposit = isDeposit;
            Amount = amount;
            AccountId = account.ID;
            BudgetId = budget?.ID;
            TransactionDate = System.DateTime.Now;
        }
    }
}
