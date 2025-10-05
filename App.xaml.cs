using FinancialCalculator.Models;
using FinancialCalculator.Stores;
using FinancialCalculator.ViewModels;
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
            FinancialInstitutionsStore _financialInstitutionsStore = new FinancialInstitutionsStore();
            BudgetStore _budgetsStore = new BudgetStore();

            

            MainWindow = new MainWindow() { DataContext = new MainWindowViewModel(_navigationStore, _financialInstitutionsStore, _budgetsStore) };
            MainWindow.Show();

            base.OnStartup(e);
        }

    }

}
