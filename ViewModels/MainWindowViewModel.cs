


namespace FinancialCalculator.ViewModels
{
    internal class MainWindowViewModel
    {

        public ViewModelBase CurrentViewModel { get; set; }


        public MainWindowViewModel() 
        { 
            CurrentViewModel = new CalculatorViewModel();
        }
    }
}
