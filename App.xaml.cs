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
            _navigationStore.CurrentViewModel = new DepositCalculatorViewModel();

            _financialInstitutionsStore.AddFinancialInstitution(new FinancialInstitution("USAA"));
            _financialInstitutionsStore.AddFinancialInstitution(new FinancialInstitution("Discover"));
            _financialInstitutionsStore.AddFinancialInstitution(new FinancialInstitution("Fidelity"));

            MainWindow = new MainWindow() { DataContext = new MainWindowViewModel(_navigationStore, _financialInstitutionsStore) };
            MainWindow.Show();

            base.OnStartup(e);
        }

    }

}
