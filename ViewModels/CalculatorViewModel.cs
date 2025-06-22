using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using FinancialCalculator.Model;
using FinancialCalculator.Stores;
using FinancialCalculator.ViewModels.Models;

namespace FinancialCalculator.ViewModel
{
    internal class CalculatorViewModel : ViewModelBase
    {
        public float PaycheckAmount
        {
            get => _paycheck.PaycheckAmount;
            set { _paycheck.PaycheckAmount = value; UpdateCalculatedValues(); }
        }

        public float TakeHomeAmount { get => _paycheck.TakeHomeAmount; }


        #region Taxes

        public float TotalTaxAmount
        {
            get
            {
                return _paycheck is null ? 0f : _paycheck.FederalTaxAmount + _paycheck.StateTaxAmount;
            }
        }

        public float TotalTaxPercent { get => TotalTaxAmount / PaycheckAmount; }
        public float FederalTaxPercent { get => FederalTaxAmount / PaycheckAmount; }
        public float StateTaxPercent { get => StateTaxAmount / PaycheckAmount; }

        public float FederalTaxAmount
        {
            get => _paycheck is null ? 0f : _paycheck.FederalTaxAmount;
            set
            {
                _paycheck.FederalTaxAmount = value; UpdateCalculatedValues();
            }
        }

        public float StateTaxAmount
        {
            get => _paycheck is null ? 0f : _paycheck.StateTaxAmount;
            set { _paycheck.StateTaxAmount = value; UpdateCalculatedValues(); }
        }


        #endregion


        private PaycheckStore _paycheck;


        private ObservableCollection<BalanceSheetViewModel> balanceSheets = new ObservableCollection<BalanceSheetViewModel>();
        public ObservableCollection<BalanceSheetViewModel> BalanceSheets { get => balanceSheets; set { balanceSheets = value; UpdateCalculatedValues(); } }


        public CalculatorViewModel()
        {
            _paycheck = new PaycheckStore();

            BalanceSheets.Add(new BalanceSheetViewModel(_paycheck, "Investments"));
            BalanceSheets.Add(new BalanceSheetViewModel(_paycheck, "Fixed Costs"));
            BalanceSheets.Add(new SavingsBalanceSheetViewModel(_paycheck, "Savings"));
            BalanceSheets.Add(new BalanceSheetViewModel(_paycheck, "Free Spending"));

            BalanceSheets[0].AddBalanceSheetItem(new BalanceItem(_paycheck, "401K", _monthlyAmount: 933.36f));
            BalanceSheets[0].AddBalanceSheetItem(new BalanceItem(_paycheck, "RothIRA", _monthlyPercent: 0.05f));
            BalanceSheets[0].AddBalanceSheetItem(new BalanceItem(_paycheck, "HSA"));

            BalanceSheets[1].AddBalanceSheetItem(new BalanceItem(_paycheck, "Studen Loans"));
            BalanceSheets[1].AddBalanceSheetItem(new BalanceItem(_paycheck, "Motorcycle Insurance"));
            BalanceSheets[1].AddBalanceSheetItem(new BalanceItem(_paycheck, "AMO Dues"));

            BalanceSheets[2].AddBalanceSheetItem(new SavingsBalanceItem(_paycheck, "Emergency Fund"));
            BalanceSheets[2].AddBalanceSheetItem(new SavingsBalanceItem(_paycheck, "Rally Car"));
            BalanceSheets[2].AddBalanceSheetItem(new SavingsBalanceItem(_paycheck, "House"));

            BalanceSheets[3].AddBalanceSheetItem(new BalanceItem(_paycheck, "Food"));
            BalanceSheets[3].AddBalanceSheetItem(new BalanceItem(_paycheck, "Gas"));
            BalanceSheets[3].AddBalanceSheetItem(new BalanceItem(_paycheck, "Food"));
            BalanceSheets[3].AddBalanceSheetItem(new BalanceItem(_paycheck, "Spotify", _monthlyAmount: 12.71f));
            BalanceSheets[3].AddBalanceSheetItem(new BalanceItem(_paycheck, "Adobe", _monthlyAmount: 15.89f));
            BalanceSheets[3].AddBalanceSheetItem(new BalanceItem(_paycheck, "Google Photos", _monthlyAmount: 2.11f));
            BalanceSheets[3].AddBalanceSheetItem(new BalanceItem(_paycheck, "Other"));

        }



        public void UpdateCalculatedValues()
        {
            OnPropertyChanged("FederalTaxPercent");
            OnPropertyChanged("StateTaxPercent");
            OnPropertyChanged("TotalTaxAmount");
            OnPropertyChanged("TotalTaxPercent");
            OnPropertyChanged("TakeHomeAmount");

            OnPropertyChanged("BalanceSheets");

        }
    }

}
