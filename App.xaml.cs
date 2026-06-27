using FinancialCalculator.Data;
using FinancialCalculator.Models;
using FinancialCalculator.Stores;
using FinancialCalculator.ViewModels;
using System.IO;
using System.Windows;

namespace FinancialCalculator
{
    public partial class App : Application
    {
        public App()
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Ensure the data directory exists
            string dataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "FinancialCalculator");
            Directory.CreateDirectory(dataDir);
            string dbPath = Path.Combine(dataDir, "data.db");

            var db = new FinancialDbContext(dbPath);
            DatabaseInitializer.EnsureCreated(db);

            // Load data from DB
            var (institutions, accountsById) = DatabaseInitializer.LoadAccountsAndInstitutions(db);
            var (activeBudgets, hiddenBudgets) = DatabaseInitializer.LoadBudgets(db, accountsById);

            NavigationStore navigationStore = new NavigationStore();

            FinancialInstitutionsStore financialInstitutionsStore = new FinancialInstitutionsStore(
                db,
                financialInstitutions: institutions,
                financialAccounts: accountsById.Values.ToList());

            BudgetStore budgetStore = new BudgetStore(financialInstitutionsStore, db,
                budgets: activeBudgets,
                hiddenBudgets: hiddenBudgets);

            DepositStore depositStore = new DepositStore(budgetStore);

            MainWindow = new MainWindow()
            {
                DataContext = new MainWindowViewModel(navigationStore, financialInstitutionsStore, budgetStore, depositStore)
            };
            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}
