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
            _navigationStore.CurrentViewModel = new CalculatorViewModel();

            MainWindow = new MainWindow() { DataContext = new MainWindowViewModel(_navigationStore) };
            MainWindow.Show();

            base.OnStartup(e);
        }

    }

}
