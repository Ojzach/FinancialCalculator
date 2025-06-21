using FinancialCalculator.Stores;
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

        protected float monthlyAmount = 0;
        protected float monthlyPercent = 0;
        protected bool setByAmount = true;

        public float MonthlyAmount { 
            get => monthlyAmount; 
            set {
                setByAmount = true;
                monthlyAmount = value; 
                monthlyPercent = monthlyAmount / _paycheck.TakeHomeAmount;
                OnPropertyChanged("MonthlyAmount");
                OnPropertyChanged("MonthlyPercentStr");
                NumbersChanged?.Invoke();
            } 
        }
        public string MonthlyPercentStr
        {
            get => (monthlyPercent * 100).ToString("0.00") + "%"; 
            set {
                setByAmount = false;
                float p = float.Parse(value.Trim(new Char[] { '%' }));
                monthlyPercent = p <= 100 ? p / 100 : 1;
                monthlyAmount = monthlyPercent * _paycheck.TakeHomeAmount;
                OnPropertyChanged("MonthlyAmount");
                OnPropertyChanged("MonthlyPercentStr");
                NumbersChanged?.Invoke();
            }
        }
        public string Notes { get; set; }


        private PaycheckStore _paycheck;

        public BalanceItem(PaycheckStore paycheck, string name, float _monthlyAmount = -1.0f, float _monthlyPercent = -1.0f, string notes = "") 
        {
            _paycheck = paycheck;
            Name = name;
            if (_monthlyAmount != -1) MonthlyAmount = _monthlyAmount;
            else if (_monthlyPercent != -1) MonthlyPercentStr = (_monthlyPercent * 100).ToString();
            Notes = notes;

            _paycheck.PaycheckChanged += UpdateNumbers;
        }

        private void UpdateNumbers()
        {
            if (setByAmount) MonthlyAmount = monthlyAmount;
            else MonthlyPercentStr = (monthlyPercent * 100).ToString();
        }


        public Action NumbersChanged;
    }
}
