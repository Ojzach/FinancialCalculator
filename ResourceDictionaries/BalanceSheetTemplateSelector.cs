using FinancialCalculator.ViewModels;
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
                if (item is SavingsBalanceSheetViewModel) return element.FindResource("SavingsBalanceSheet") as DataTemplate;
                else if (item is BalanceSheetBaseViewModel) return element.FindResource("BalanceSheet") as DataTemplate;
            }


            return base.SelectTemplate(item, container);

        }
    }
}
