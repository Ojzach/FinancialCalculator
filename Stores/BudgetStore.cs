using FinancialCalculator.Models;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Stores
{
    internal class BudgetStore
    {
        public Dictionary<int, Budget> Budgets { get => budgets; }
        public Dictionary<int, Budget> HiddenBudgets { get => hiddenBudgets; }


        Dictionary<int, Budget> budgets = new Dictionary<int, Budget>();
        Dictionary<int, Budget> hiddenBudgets = new Dictionary<int, Budget>();

        public BudgetStore()
        {

            FinancialAccount postTax = new FinancialAccount("Post Tax", BankAccountType.Checking, 0);
            FinancialAccount preTax = new FinancialAccount("Pre Tax", BankAccountType.Checking, 0);
            preTax.isPreTaxAccount = true;

            hiddenBudgets.Add(-2, new FixedBudget(-2, "Federal Income Tax", BudgetPriority.VeryHigh, preTax));
            hiddenBudgets.Add(-3, new FixedBudget(-3, "Medicare", BudgetPriority.VeryHigh, preTax));
            hiddenBudgets.Add(-4, new FixedBudget(-4, "Social Security", BudgetPriority.VeryHigh, preTax));
            hiddenBudgets.Add(-5, new FixedBudget(-5, "State Income Tax", BudgetPriority.VeryHigh, preTax));


            Budgets.Add(1, new FixedBudget(1, "Investments", BudgetPriority.VeryHigh, postTax, setPct: 0.6f));
            Budgets.Add(2, new FixedBudget(2, "401K", BudgetPriority.VeryHigh, preTax, setPct: 0.1f));
            Budgets.Add(3, new FixedBudget(3, "Roth IRA", BudgetPriority.Medium, postTax, setPct: 0.1f));
            Budgets.Add(4, new FixedBudget(4, "HSA", BudgetPriority.Low, postTax));
            Budgets.Add(5, new SavingsBudget(5, "House", BudgetPriority.Medium, postTax, 50000, new LocalDate(2026, 10, 20)));
            Budgets[1].AddChildBudget(new List<int> { 2, 3, 4, 5 });

            Budgets.Add(6, new FlexibleBudget(6, "Fixed Costs", BudgetPriority.VeryHigh, postTax));
            Budgets.Add(7, new RecurringExpenseBudget(7, "Student Loans", BudgetPriority.VeryHigh, postTax, 500f));
            Budgets.Add(8, new RecurringExpenseBudget(8, "Motorcycle Insurance", BudgetPriority.High, postTax, 43.32f));
            Budgets.Add(9, new RecurringExpenseBudget(9, "AMO Dues", BudgetPriority.Medium, postTax, 425, 3));
            Budgets[6].AddChildBudget(new List<int> { 7, 8, 9 });

            Budgets.Add(10, new FixedBudget(10, "Savings", BudgetPriority.Medium, postTax, setPct: 0.2f));
            Budgets.Add(11, new SavingsBudget(11, "Emergency Fund", BudgetPriority.VeryHigh, postTax, 10000, new LocalDate(2025, 8, 25)));
            Budgets.Add(12, new SavingsBudget(12, "Rally Car", BudgetPriority.VeryLow, postTax, 15000, new LocalDate(2026, 3, 1)));
            Budgets.Add(13, new SavingsBudget(13,"Travel", BudgetPriority.Low, postTax, 3000, new LocalDate(2025, 8, 20)));
            Budgets[10].AddChildBudget(new List<int> { 11, 12, 13 });

            Budgets.Add(14, new FlexibleBudget(14, "Free Spending", BudgetPriority.Low, postTax));
            Budgets.Add(15, new FlexibleBudget(15, "Food", BudgetPriority.High, postTax));
            Budgets.Add(16, new FlexibleBudget(16, "Gas", BudgetPriority.Medium, postTax));
            Budgets.Add(17, new RecurringExpenseBudget(17, "Spotify", BudgetPriority.VeryHigh, postTax, 12.71f));
            Budgets.Add(18, new RecurringExpenseBudget(18, "Adobe", BudgetPriority.VeryHigh, postTax, 15.89f));
            Budgets.Add(19, new RecurringExpenseBudget(19, "Google Photos", BudgetPriority.VeryHigh, postTax, 2.11f));
            Budgets[14].AddChildBudget(new List<int> { 15, 16, 17, 18, 19 });
        }

        public Budget GetBudget(int budgetID)
        {
            if(budgets.ContainsKey(budgetID)) return budgets[budgetID];
            else if(hiddenBudgets.ContainsKey(budgetID)) return hiddenBudgets[budgetID];

            throw new Exception("Budget ID does not exist in the Budget Store");
        }

        public bool IsBudgetPreTax(int budgetID) => budgets[budgetID].AssociatedFinancialAccount.isPreTaxAccount;
    }
}
