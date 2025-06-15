using System.Configuration;
using System.Data;
using System.Windows;
using FinancialCalculator.ViewModel;

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

            MainWindow = new MainWindow() { DataContext = new MainWindowViewModel() };
            MainWindow.Show();

            base.OnStartup(e);
        }

    }

}
