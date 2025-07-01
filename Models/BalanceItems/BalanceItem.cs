using FinancialCalculator.Commands;
using FinancialCalculator.Models;
using FinancialCalculator.Stores;
using FinancialCalculator.ViewModels;
using System.Diagnostics;

namespace FinancialCalculator.Model
{
    internal class BalanceItem : ViewModelBase
    {

        public string Name { get; set; }

        protected float monthlyAmt = 0;
        protected float monthlyPct = 0;
        protected bool setByAmt = true;

        protected BankAccount bankAccount;

        public float MonthlyAmt { 
            get => monthlyAmt; 
            set {
                SetAmountAndPercent(amount: value);
                NumbersChanged?.Invoke();
            } 
        }

        public float AdjMonthlyAmt
        {
            get => monthlyAmt * _paycheck.MonthsCoveredByPaycheck;
        }
        public string MonthlyPctStr
        {
            get => (monthlyPct * 100).ToString("0.00") + "%"; 
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
            if (_monthlyAmount != -1) MonthlyAmt = _monthlyAmount;
            else if (_monthlyPercent != -1) MonthlyPctStr = (_monthlyPercent * 100).ToString();
            Notes = notes;

            _paycheck.PaycheckChanged += UpdateNumbers;
        }

        private void UpdateNumbers()
        {
            if (setByAmt) MonthlyAmt = monthlyAmt;
            else MonthlyPctStr = (monthlyPct * 100).ToString();
        }

        public void SetAmountAndPercent(float amount = -1, float percent = -1)
        {

            if(amount != -1)
            {
                setByAmt = true;
                monthlyAmt = amount;
                monthlyPct = monthlyAmt / _paycheck.TakeHomeAmount;
            }
            else if(percent != -1)
            {
                setByAmt = false;
                monthlyPct = percent;
                monthlyAmt = monthlyPct * _paycheck.TakeHomeAmount;
            }

            OnPropertyChanged(nameof(MonthlyAmt));
            OnPropertyChanged(nameof(MonthlyPctStr));
        }


        public Action NumbersChanged;
    }
}
