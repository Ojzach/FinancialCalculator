using FinancialCalculator.Models;
using FinancialCalculator.Stores;
using FinancialCalculator.ViewModels;
using NodaTime;
using System.Windows;


namespace FinancialCalculator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {

            NavigationStore _navigationStore = new NavigationStore();


            FinancialInstitutionsStore _financialInstitutionsStore = new FinancialInstitutionsStore(
                financialInstitutions: new List<FinancialInstitution>()
                {
                    new FinancialInstitution("USAA"),
                    new FinancialInstitution("Discover"),
                    new FinancialInstitution("Fidelity")
                },
                financialAccounts: new List<FinancialAccount>()
                { 
                    new FinancialAccount(0, "Income", 0, BankAccountType.Checking, 2000),
                    new FinancialAccount(1, "Spending", 0, BankAccountType.Checking, 1000),
                    new FinancialAccount(2, "Credit Card", 0, BankAccountType.Credit, -1000),
                    new FinancialAccount(3, "Emergency Fund", 1, BankAccountType.Savings, 10000),
                    new FinancialAccount(4, "State Tax", 1, BankAccountType.Savings, 5000, _isPreTaxAccount: true),
                    new FinancialAccount(5, "Roth IRA", 2, BankAccountType.Investment, 10000),
                    new FinancialAccount(6, "401K", 2, BankAccountType.Investment, 10000, _isPreTaxAccount: true)
                }
                )
            {

            };


            BudgetStore _budgetsStore = new BudgetStore(_financialInstitutionsStore,
                budgets: new List<Budget>()
                {
                    //new FixedBudget(0, "Base Budget", BudgetPriority.VeryHigh, _financialInstitutionsStore.GetFinancialAccount(0), setPct: 1, childBudgets: [1, 6, 10, 14]),

                    new FixedBudget(1, "Investments", BudgetPriority.VeryHigh, _financialInstitutionsStore.GetFinancialAccount(0), setPct: 0.6m, childBudgets: [2, 3, 5]),
                    new FixedBudget(2, "401K", BudgetPriority.VeryHigh, _financialInstitutionsStore.GetFinancialAccount(6), setPct: 0.1m),
                    new FixedBudget(3, "Roth IRA", BudgetPriority.Medium, _financialInstitutionsStore.GetFinancialAccount(5), setPct: 0.1m),
                    new SavingsBudget(5, "House", BudgetPriority.Medium, _financialInstitutionsStore.GetFinancialAccount(0), 50000, new LocalDate(2026, 10, 20)),

                    new FlexibleBudget(6, "Fixed Costs", BudgetPriority.VeryHigh, _financialInstitutionsStore.GetFinancialAccount(1), childBudgets: [7, 8, 9]),
                    new RecurringExpenseBudget(7, "Student Loans", BudgetPriority.VeryHigh, _financialInstitutionsStore.GetFinancialAccount(1), 500m),
                    new RecurringExpenseBudget(8, "Motorcycle Insurance", BudgetPriority.High, _financialInstitutionsStore.GetFinancialAccount(1), 43.32m),
                    new RecurringExpenseBudget(9, "AMO Dues", BudgetPriority.Medium, _financialInstitutionsStore.GetFinancialAccount(1), 425, 3),

                    new FixedBudget(10, "Savings", BudgetPriority.Medium, _financialInstitutionsStore.GetFinancialAccount(0), setPct: 0.2m, childBudgets: [11, 12, 13]),
                    new SavingsBudget(11, "Emergency Fund", BudgetPriority.VeryHigh, _financialInstitutionsStore.GetFinancialAccount(3), 10000, new LocalDate(2025, 8, 25)),
                    new SavingsBudget(12, "Rally Car", BudgetPriority.VeryLow, _financialInstitutionsStore.GetFinancialAccount(0), 15000, new LocalDate(2026, 3, 1)),
                    new SavingsBudget(13, "Travel", BudgetPriority.Low, _financialInstitutionsStore.GetFinancialAccount(0), 3000, new LocalDate(2025, 8, 20)),

                    new FlexibleBudget(14, "Free Spending", BudgetPriority.Low, _financialInstitutionsStore.GetFinancialAccount(1), [15, 16, 17, 18, 19]),
                    new FlexibleBudget(15, "Food", BudgetPriority.High, _financialInstitutionsStore.GetFinancialAccount(1)),
                    new FlexibleBudget(16, "Gas", BudgetPriority.Medium, _financialInstitutionsStore.GetFinancialAccount(1)),
                    new RecurringExpenseBudget(17, "Spotify", BudgetPriority.VeryHigh, _financialInstitutionsStore.GetFinancialAccount(1), 12.71m),
                    new RecurringExpenseBudget(18, "Adobe", BudgetPriority.VeryHigh, _financialInstitutionsStore.GetFinancialAccount(1), 15.89m),
                    new RecurringExpenseBudget(19, "Google Photos", BudgetPriority.VeryHigh, _financialInstitutionsStore.GetFinancialAccount(1), 2.11m)
                },
                hiddenBudgets: new List<Budget>()
                {
                    new FixedBudget(-2, "Federal Income Tax", BudgetPriority.VeryHigh,  _financialInstitutionsStore.GetFinancialAccount(4), setPct: 0.145m),
                    new FixedBudget(-3, "Medicare", BudgetPriority.VeryHigh,  _financialInstitutionsStore.GetFinancialAccount(4), setPct: 0.0145m),
                    new FixedBudget(-4, "Social Security", BudgetPriority.VeryHigh,  _financialInstitutionsStore.GetFinancialAccount(4), setPct: 0.062m),
                    new FixedBudget(-5, "State Income Tax", BudgetPriority.VeryHigh,  _financialInstitutionsStore.GetFinancialAccount(4), setPct: 0m)
                });



            MainWindow = new MainWindow() { DataContext = new MainWindowViewModel(_navigationStore, _financialInstitutionsStore, _budgetsStore) };
            MainWindow.Show();

            base.OnStartup(e);
        }

    }

}
