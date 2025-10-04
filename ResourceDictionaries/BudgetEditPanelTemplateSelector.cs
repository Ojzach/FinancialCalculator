using FinancialCalculator.Models;
using FinancialCalculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FinancialCalculator.ResourceDictionaries
{
    internal class BudgetEditPanelTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null)
            {

                if (item is BudgetViewModel)
                {
                    if (item is SavingsBudgetViewModel) return element.FindResource("SavingsBudget_EditPanelTemplate") as DataTemplate;
                    else if (item is FixedBudgetViewModel) return element.FindResource("FixedBudget_EditPanelTemplate") as DataTemplate;
                }
            }

            return null;
        }
    }
}
