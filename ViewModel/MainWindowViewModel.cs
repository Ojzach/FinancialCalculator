using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinancialCalculator.ViewModel;

namespace FinancialCalculator.ViewModel
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
