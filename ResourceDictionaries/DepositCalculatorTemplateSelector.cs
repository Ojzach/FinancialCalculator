using FinancialCalculator.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace FinancialCalculator.ResourceDictionaries
{
    class DepositCalculatorTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = (FrameworkElement)container;
            if (element != null)
            {
                if (item is DepositCalculatorBudgetViewModel) return element.FindResource("DepositBudget") as DataTemplate;
                else if (item is TransactionViewModel) return element.FindResource("DepositItem") as DataTemplate;
            }


            return base.SelectTemplate(item, container);

        }
    }
}
