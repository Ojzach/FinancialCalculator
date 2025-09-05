using FinancialCalculator.Models;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Stores
{
    internal class BudgetsStore
    {
        List<Budget> budgets = new List<Budget>();
        public List<Budget> Budgets { get { return budgets; } }

        private Budget baseBudget;
        public Budget BaseBudget { get => baseBudget; set { baseBudget = value; } }


        public BudgetsStore()
        {

            FinancialAccount postTax = new FinancialAccount("Post Tax", BankAccountType.Checking, 0);
            FinancialAccount preTax = new FinancialAccount("Pre Tax", BankAccountType.Checking, 0);
            preTax.isPreTaxAccount = true;

            BaseBudget = new FixedBudget("Deposit", postTax);

            BaseBudget.ChildBudgets.Add(new FixedBudget("Investments", postTax, false, setPct: 0.6f));
            BaseBudget.ChildBudgets[BaseBudget.ChildBudgets.Count - 1].ChildBudgets.Add(new FixedBudget("401K", preTax, false, setPct: 0.1f));
            BaseBudget.ChildBudgets[BaseBudget.ChildBudgets.Count - 1].ChildBudgets.Add(new FixedBudget("Roth IRA", postTax, false, setPct: 0.1f));
            BaseBudget.ChildBudgets[BaseBudget.ChildBudgets.Count - 1].ChildBudgets.Add(new FixedBudget("HSA", postTax));
            BaseBudget.ChildBudgets[BaseBudget.ChildBudgets.Count - 1].ChildBudgets.Add(new SavingsBudget("House", postTax, 50000, new LocalDate(2026, 10, 20), BudgetPriority.Medium));
            BaseBudget.ChildBudgets.Add(new FlexibleBudget("Fixed Costs", postTax));
            BaseBudget.ChildBudgets[BaseBudget.ChildBudgets.Count - 1].ChildBudgets.Add(new RecurringExpenseBudget("Student Loans", postTax, 500f));
            BaseBudget.ChildBudgets[BaseBudget.ChildBudgets.Count - 1].ChildBudgets.Add(new RecurringExpenseBudget("Motorcycle Insurance", postTax, 43.32f));
            BaseBudget.ChildBudgets[BaseBudget.ChildBudgets.Count - 1].ChildBudgets.Add(new RecurringExpenseBudget("AMO Dues", postTax, 425, 3));
            BaseBudget.ChildBudgets.Add(new FixedBudget("Savings", postTax, false, setPct: 0.2f));
            BaseBudget.ChildBudgets[BaseBudget.ChildBudgets.Count - 1].ChildBudgets.Add(new SavingsBudget("Emergency Fund", postTax, 10000, new LocalDate(2025, 8, 25), BudgetPriority.High));
            BaseBudget.ChildBudgets[BaseBudget.ChildBudgets.Count - 1].ChildBudgets.Add(new SavingsBudget("Rally Car", postTax, 15000, new LocalDate(2026, 3, 1)));
            BaseBudget.ChildBudgets[BaseBudget.ChildBudgets.Count - 1].ChildBudgets.Add(new SavingsBudget("Travel", postTax, 3000, new LocalDate(2025, 8, 20), BudgetPriority.High));
            BaseBudget.ChildBudgets.Add(new FlexibleBudget("Free Spending", postTax));
            BaseBudget.ChildBudgets[BaseBudget.ChildBudgets.Count - 1].ChildBudgets.Add(new FlexibleBudget("Food", postTax));
            BaseBudget.ChildBudgets[BaseBudget.ChildBudgets.Count - 1].ChildBudgets.Add(new FlexibleBudget("Gas", postTax));
            BaseBudget.ChildBudgets[BaseBudget.ChildBudgets.Count - 1].ChildBudgets.Add(new RecurringExpenseBudget("Spotify", postTax, 12.71f));
            BaseBudget.ChildBudgets[BaseBudget.ChildBudgets.Count - 1].ChildBudgets.Add(new RecurringExpenseBudget("Adobe", postTax, 15.89f));
            BaseBudget.ChildBudgets[BaseBudget.ChildBudgets.Count - 1].ChildBudgets.Add(new RecurringExpenseBudget("Google Photos", postTax, 2.11f));
        }
    }
}
