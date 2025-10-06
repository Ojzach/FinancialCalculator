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

            hiddenBudgets.Add(-2, new FixedBudget(-2, "Federal Income Tax", preTax));
            hiddenBudgets.Add(-3, new FixedBudget(-3, "Medicare", preTax));
            hiddenBudgets.Add(-4, new FixedBudget(-4, "Social Security", preTax));
            hiddenBudgets.Add(-5, new FixedBudget(-5, "State Income Tax", preTax));


            Budgets.Add(1, new FixedBudget(1, "Investments", postTax, setPct: 0.6f));
            Budgets.Add(2, new FixedBudget(2, "401K", preTax, setPct: 0.1f));
            Budgets.Add(3, new FixedBudget(3, "Roth IRA", postTax, setPct: 0.1f));
            Budgets.Add(4, new FixedBudget(4, "HSA", postTax));
            Budgets.Add(5, new SavingsBudget(5, "House", postTax, 50000, new LocalDate(2026, 10, 20), BudgetPriority.Medium));
            Budgets[1].AddChildBudget(new List<int> { 2, 3, 4, 5 });

            Budgets.Add(6, new FlexibleBudget(6, "Fixed Costs", postTax));
            Budgets.Add(7, new RecurringExpenseBudget(7, "Student Loans", postTax, 500f));
            Budgets.Add(8, new RecurringExpenseBudget(8, "Motorcycle Insurance", postTax, 43.32f));
            Budgets.Add(9, new RecurringExpenseBudget(9, "AMO Dues", postTax, 425, 3));
            Budgets[6].AddChildBudget(new List<int> { 7, 8, 9 });

            Budgets.Add(10, new FixedBudget(10, "Savings", postTax, setPct: 0.2f));
            Budgets.Add(11, new SavingsBudget(11, "Emergency Fund", postTax, 10000, new LocalDate(2025, 8, 25), BudgetPriority.High));
            Budgets.Add(12, new SavingsBudget(12, "Rally Car", postTax, 15000, new LocalDate(2026, 3, 1)));
            Budgets.Add(13, new SavingsBudget(13,"Travel", postTax, 3000, new LocalDate(2025, 8, 20), BudgetPriority.High));
            Budgets[10].AddChildBudget(new List<int> { 11, 12, 13 });

            Budgets.Add(14, new FlexibleBudget(14, "Free Spending", postTax));
            Budgets.Add(15, new FlexibleBudget(15, "Food", postTax));
            Budgets.Add(16, new FlexibleBudget(16, "Gas", postTax));
            Budgets.Add(17, new RecurringExpenseBudget(17, "Spotify", postTax, 12.71f));
            Budgets.Add(18, new RecurringExpenseBudget(18, "Adobe", postTax, 15.89f));
            Budgets.Add(19, new RecurringExpenseBudget(19, "Google Photos", postTax, 2.11f));
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
