using FinancialCalculator.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialCalculator.Model
{
    internal class BalanceItem : ViewModelBase
    {

        public string Name { get; set; }

        protected float monthlyAmount;
        protected float monthlyPercent;

        public float MonthlyAmount { 
            get => monthlyAmount; 
            set { 
                monthlyAmount = value; 
                monthlyPercent = monthlyAmount / balanceSheet.takeHomeAmount;
                OnPropertyChanged("MonthlyAmount");
                OnPropertyChanged("MonthlyPercentStr");
                balanceSheet.UpdateCalculatedValues();
            } 
        }
        public string MonthlyPercentStr
        {
            get => (monthlyPercent * 100).ToString("0.00") + "%"; 
            set {
                float p = float.Parse(value.Trim(new Char[] { '%' }));
                monthlyPercent =  p / 100;
                monthlyAmount = monthlyPercent * balanceSheet.takeHomeAmount;
                OnPropertyChanged("MonthlyAmount");
                OnPropertyChanged("MonthlyPercentStr");
                balanceSheet.UpdateCalculatedValues();
            }
        }
        public string Notes { get; set; }


        private BalanceSheet balanceSheet;

        public BalanceItem(BalanceSheet _balanceSheet, string name, float _monthlyAmount = -1.0f, float _monthlyPercent = -1.0f, string notes = "") 
        {
            if (_balanceSheet == null) throw new ArgumentNullException("Balance Sheet Cannot Be Null");

            balanceSheet = _balanceSheet;
            Name = name;
            monthlyAmount = _monthlyAmount;
            monthlyPercent = _monthlyPercent;
            Notes = notes;

            if (monthlyAmount != -1) monthlyPercent = monthlyAmount / balanceSheet.takeHomeAmount;
            else if (monthlyPercent != -1.0f) monthlyAmount = monthlyPercent * balanceSheet.takeHomeAmount;
            else
            {
                monthlyAmount = 0;
                monthlyPercent = 0;
            }
        }
    }
}
