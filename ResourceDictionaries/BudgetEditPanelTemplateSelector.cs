using FinancialCalculator.Models;
using FinancialCalculator.ViewModels;
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
    internal class BudgetEditPanelTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element is null || item is null || item is not BudgetViewModel) return null;

            return item switch
            {
                SavingsBudgetViewModel => element.FindResource("SavingsBudget_EditPanelTemplate") as DataTemplate,
                RecurringExpenseBudgetViewModel => element.FindResource("RecurringExpenseBudget_EditPanelTemplate") as DataTemplate,
                FlexibleBudgetViewModel => element.FindResource("FlexibleBudget_EditPanelTemplate") as DataTemplate,
                FixedBudgetViewModel => element.FindResource("FixedBudget_EditPanelTemplate") as DataTemplate,
                _ => null
            };
        }
    }
}
