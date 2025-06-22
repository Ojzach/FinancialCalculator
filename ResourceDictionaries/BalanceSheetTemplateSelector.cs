using FinancialCalculator.ViewModel;
using FinancialCalculator.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FinancialCalculator.ResourceDictionaries
{
    class BalanceSheetTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = (FrameworkElement)container;
            if(element != null)
            {
                if (item is SavingsBalanceSheetViewModel)
                {
                    Debug.Print("Hi");
                    return element.FindResource("SavingsBalanceSheet") as DataTemplate;
                }
                else if (item is BalanceSheetViewModel) return element.FindResource("BalanceSheet") as DataTemplate;
            }


            return base.SelectTemplate(item, container);

        }
    }
}
