using FinancialCalculator.Models;
using FinancialCalculator.Stores;
using FinancialCalculator.ViewModels;
using System.Diagnostics;

namespace FinancialCalculator.Model
{
    internal class BalanceItem : ViewModelBase
    {

        public string Name { get; set; }

        protected float monthlyAmount = 0;
        protected float monthlyPercent = 0;
        protected bool setByAmount = true;

        protected BankAccount bankAccount;

        public float MonthlyAmount { 
            get => monthlyAmount; 
            set {
                SetAmountAndPercent(amount: value);
                NumbersChanged?.Invoke();
            } 
        }
        public string MonthlyPercentStr
        {
            get => (monthlyPercent * 100).ToString("0.00") + "%"; 
            set {
                float p = float.Parse(value.Trim(new Char[] { '%' }));
                SetAmountAndPercent(percent: p <= 100 ? p / 100 : 1);
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

        public void SetAmountAndPercent(float amount = -1, float percent = -1)
        {

            if(amount != -1)
            {
                setByAmount = true;
                monthlyAmount = amount;
                monthlyPercent = monthlyAmount / _paycheck.TakeHomeAmount;
            }
            else if(percent != -1)
            {
                setByAmount = false;
                monthlyPercent = percent;
                monthlyAmount = monthlyPercent * _paycheck.TakeHomeAmount;
            }

            OnPropertyChanged("MonthlyAmount");
            OnPropertyChanged("MonthlyPercentStr");
        }


        public Action NumbersChanged;
    }
}
