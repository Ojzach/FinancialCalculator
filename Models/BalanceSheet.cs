using System.Collections.ObjectModel;

namespace FinancialCalculator.Model
{
    internal class BalanceSheet
    {

        public string name;

        public ObservableCollection<BalanceItem> BalanceItems = new ObservableCollection<BalanceItem>();

        public float maxUsagePercent;
        public bool isPreTax = false;

        public BalanceSheet(string _name, float _maxUsagePercent = 1.00f, bool _isPreTax = false) 
        {
            name = _name;
            maxUsagePercent = _maxUsagePercent;
            isPreTax = _isPreTax;
        }
    }
}
